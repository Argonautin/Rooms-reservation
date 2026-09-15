(ns role-classifier.core
  (:require
   [org.httpkit.server :as http]
   [reitit.ring :as ring]
   [ring.middleware.json :refer [wrap-json-body wrap-json-response]]
   [ring.middleware.params :refer [wrap-params]]
   [clojure.string :as str]))

;; Dozwolone domeny instytucjonalne SAN.
(def professor-domains
  #{"san.edu.pl"})

(def student-domains
  #{"student.san.edu.pl"})

;; Niemutowalna kolekcja reguł klasyfikacji.
;; Kolejność jest istotna: student.san.edu.pl również kończy się na san.edu.pl,
;; dlatego reguła studenta występuje przed regułą profesora.
(def role-rules
  [{:role "student"
    :domains student-domains
    :confidence 0.95
    :reason "Adres e-mail należy do domeny studenckiej SAN."}

   {:role "professor"
    :domains professor-domains
    :confidence 0.95
    :reason "Adres e-mail należy do domeny pracowniczej SAN."}])

;; Funkcja czysta: otrzymuje e-mail i zwraca jego domenę.
;; Przykład: "Jan.Kowalski@student.san.edu.pl" -> "student.san.edu.pl".
(defn email-domain [email]
  (when (and (string? email) (str/includes? email "@"))
    (-> email
        str/trim
        str/lower-case
        (str/split #"@")
        last)))

(defn domain-matches-rule? [domain rule]
  (contains? (:domains rule) domain))

;; filter: zwraca tylko reguły pasujące do domeny.
(defn matching-rules [domain]
  (filter
   (fn [rule]
     (domain-matches-rule? domain rule))
   role-rules))

;; Zwraca pasującą regułę albo nil.
;; Nie ma domyślnej roli Student — adres spoza SAN musi być odrzucony.
(defn classify-domain [domain]
  (first (matching-rules domain)))

;; map: tworzy nową sekwencję wszystkich akceptowanych domen małymi literami.
(defn normalized-domains []
  (map
   (fn [domain]
     (str/lower-case domain))
   (concat professor-domains student-domains)))

;; reduce: buduje odpowiedź JSON z wybranych pól klasyfikacji.
(defn classification-summary [classification]
  (reduce
   (fn [summary [key value]]
     (assoc summary key value))
   {}
   (select-keys classification
                [:allowed :role :confidence :reason :email-domain])))

;; Główna funkcja klasyfikacji.
;; Zawsze zwraca mapę; dla adresu spoza SAN :allowed jest false.
(defn classify-role [{:keys [email]}]
  (let [domain (email-domain email)
        selected-rule (classify-domain domain)]
    (if selected-rule
      {:allowed true
       :role (:role selected-rule)
       :confidence (:confidence selected-rule)
       :reason (:reason selected-rule)
       :email-domain domain}
      {:allowed false
       :role nil
       :confidence 0.0
       :reason "Rejestracja jest dostępna wyłącznie dla adresów e-mail SAN."
       :email-domain domain})))

;; Pomocnicza funkcja tworząca odpowiedź HTTP.
(defn json-response [status body]
  {:status status
   :headers {"Content-Type" "application/json; charset=utf-8"}
   :body body})

;; GET http://localhost:8081/health
(defn health-handler [_]
  (json-response
   200
   {:service "rooms-role-classifier"
    :status "ok"}))

;; POST http://localhost:8081/classify-role
;;
;; Przykładowe body:
;; {
;;   "email": "jan.kowalski@student.san.edu.pl",
;;   "firstName": "Jan",
;;   "lastName": "Kowalski"
;; }
(defn classify-handler [request]
  (let [body (:body request)
        email (:email body)]
    (if (or (not (string? email))
            (str/blank? email))
      (json-response
       400
       {:allowed false
        :error "Pole email jest wymagane."})
      (json-response
       200
       (classification-summary (classify-role body))))))

(def app
  (-> (ring/ring-handler
       (ring/router
        [["/health" {:get health-handler}]
         ["/classify-role" {:post classify-handler}]]))
      wrap-params
      (wrap-json-body {:keywords? true})
      wrap-json-response))

(defn -main [& _]
  (println "Role Classifier Microservice działa na http://localhost:8081")
  (println "Health check: http://localhost:8081/health")
  (println "Klasyfikacja: POST http://localhost:8081/classify-role")
  (println "Naciśnij Ctrl+C, aby zatrzymać mikroserwis.")
  (http/run-server app {:port 8081})
  @(promise))