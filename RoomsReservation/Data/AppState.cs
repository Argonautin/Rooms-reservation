using RoomsReservation.Data.Tables;

namespace RoomsReservation.Data
{
    public class AppState
    {
        public User? CurrentUser { get; private set; }

        public event Action? OnChange;

        public void SetUser(User user)
        {
            CurrentUser = user;
            NotifyStateChanged();
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}
