using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetInfoSaloméSuvetaa
{
    public class Pixel
    {
        #region attributs
        byte r;
        byte g;
        byte b;
        #endregion

        #region propriétés
        public byte R
        {
            get { return this.r; }
            set { this.r = value; }
        }
        public byte G
        {
            get { return this.g; }
            set { this.g = value; }
        }
        public byte B
        {
            get { return this.b; }
            set { this.b = value; }
        }
        #endregion

        #region constructeur
        public Pixel(byte rouge, byte vert, byte bleu)
        {
            this.r = rouge;
            this.g = vert;
            this.b = bleu;
        }
        #endregion

        #region méthodes
        public string Tostring()
        {
            return Convert.ToString(this.r)+ "," + Convert.ToString(this.g) + "," + Convert.ToString(this.b) ;
        }
        #endregion
    }
}
