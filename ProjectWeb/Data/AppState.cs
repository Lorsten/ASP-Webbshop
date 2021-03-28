using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWeb.Data
{
    public class AppState
    {
        public event Action Onchange;

        public int CartCount { get; set; }

        public void SetCartItems(int count)
        {
            CartCount = count;
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => Onchange?.Invoke();
    }
}
