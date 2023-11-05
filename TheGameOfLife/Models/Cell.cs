using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        private bool alive;
        private bool wasAlive;
        public bool Alive 
        { 
            get {  return alive; } 
            set 
            { 
                if (alive && !value) wasAlive = true;
                alive = value;
                OnPropertyChanged();
            } 
        }
        public bool WasAlive { get { return wasAlive; } }

        public Cell(uint x, uint y, bool alive) 
        {
            X = x;
            Y = y;
            Alive = alive;
            if(alive) { this.wasAlive = true; }
            else { this.wasAlive = false; }
        }
    }
}
