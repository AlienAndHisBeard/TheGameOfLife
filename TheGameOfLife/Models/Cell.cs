using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheGameOfLife.Models
{
    /// <summary>
    /// Class representing a Cell of the game
    /// Implements INotifyPropertyChanged for Alive property
    /// </summary>
    public class Cell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string memberName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        public uint X {  get; set; }
        public uint Y { get; set; }
        private int alive;
        public int Alive 
        { 
            get {  return alive; } 
            set 
            { 
                alive = value;
                OnPropertyChanged();
            } 
        }

        public Cell(uint x, uint y, int alive) 
        {
            X = x;
            Y = y;
            Alive = alive;
        }
    }
}
