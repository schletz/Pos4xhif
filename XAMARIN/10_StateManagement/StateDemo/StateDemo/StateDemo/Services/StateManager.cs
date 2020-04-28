using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(StateDemo.Services.StateManager))]
namespace StateDemo.Services
{
    /// <summary>
    /// Verwaltet den State der Applikation, also alles wo mehrere Pages darauf Zugriff haben sollen.
    /// </summary>
    public class StateManager
    {
        // Es darf nur 1 Instanz geben
        public static StateManager Instance => DependencyService.Get<StateManager>();

        // Events: Die Viewmodels und Pages können das Event abonnieren und werden so benachrichtigt,
        // ob sich etwas geändert hat.
        // Der Typ wie Action<string> ist frei definierbar, der Abonnent muss dann mit einer
        // Lambda vom Typ (username) => {} abonnieren.
        public event Action<string> UserChanged;

        // Methoden, die verschiedene Sachen im State setzen. Geht natürlich auch über einen setter
        // im Proprety, aber da hier was ausgelöst wird, ist eine Methode intuitiver.
        public void ChangeUser(string username)
        {
            UserChanged?.Invoke(username);
        }
    }
}
