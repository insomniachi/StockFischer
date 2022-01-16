using OpenPGN.Models;
using StockFischer.Models;

namespace StockFischer
{
    public interface IViewService
    {
        void EditEngines();
        Game OpenPgn();
    }

    public class ViewService : IViewService
    {
        public void EditEngines()
        {
            throw new System.NotImplementedException();
        }

        public Game OpenPgn()
        {
            Microsoft.Win32.OpenFileDialog dialog = new();
            _ = dialog.ShowDialog();
            return Game.FromPgnFile(dialog.FileName);
        }
    }
}
