using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProjetInfoSaloméSuvetaa
{
    public class MyImage
    {
        #region attributs
        Pixel[,] image;                  //matrice de pixel contenant l'image considérée
        int width;                       //largeur de l'image
        int height;                      //hauteur de l'image
        string typeimage;                //type de l'image : ex : bmp pour bitmap
        int taillefichier;               //taille totale du fichier en octets
        int tailleoffset;                //indice correspondant à l'offset (54)
        int nbbitpcolor;                 //nombre de bit par couleur (8)
        #endregion

        #region propriétés
        public Pixel[,] Image
        {
            get { return this.image; }
            set { this.image = value; }
        }
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        public string Typeimage
        {
            get { return this.typeimage; }
            set { this.typeimage = value; }
        }
        public int Taillefichier
        {
            get { return this.taillefichier; }
            set { this.taillefichier = value; }
        }
        public int Tailleoffset
        {
            get { return this.tailleoffset; }
            set { this.tailleoffset = value; }
        }
        public int Nbbitpcolor
        {
            get { return this.nbbitpcolor; }
            set { this.nbbitpcolor = value; }
        }
        #endregion

        #region constructeurs
        public MyImage(string filename)
        {
            byte[] fichier = File.ReadAllBytes(filename);                                //on lit le fichier (l'image) et on le stocke dans un tableau "fichier"
            byte[] taillefichier = { fichier[2], fichier[3], fichier[4], fichier[5] };   //on récupère dans un tableau "taillefichier" les bytes 2 à 5 a convertir pour connaitre la taille du fichier en int
            this.taillefichier = Convertir_Endian_To_Int(taillefichier);                 //on initialise l'attribut taillefichier avec la valeur obtenue à partir de la conversion du tableau précédent
            byte[] offset = { fichier[10], fichier[11], fichier[12], fichier[13] };      //on récupère dans un tableau "offset" les bytes 10 à 13 à convertir pour savoir sur quel byte commence l'image
            this.tailleoffset = Convertir_Endian_To_Int(offset);                         //on initialise l'attribut tailleoffset avec la valeur obtenue à partir de la conversion du tableau précédent
            byte[] widthbyte = { fichier[18], fichier[19], fichier[20], fichier[21] };   //on récupère dans un tableau "widthbyte" les bytes 18 à 21 à convertir pour savoir la largeur de l'image
            this.width = Convertir_Endian_To_Int(widthbyte);                             //on initialise l'attribut width avec la valeur obtenue à partir de la conversion du tableau précédent
            byte[] height = { fichier[22], fichier[23], fichier[24], fichier[25] };      //on récupère dans un tableau "height" les bytes 22 à 25 à convertir pour savoir la hauteur de l'image
            this.height = Convertir_Endian_To_Int(height);                               //on initialise l'attribut height avec la valeur obtenue à partir de la conversion du tableau précédent
            this.typeimage = filename.Substring(filename.Length - 3);                      //on initialise l'attribut typeimage avec la chaine de caractère obtenue à partir du nom du fichier, le nom est en abrégé
            byte[] bbp = { fichier[28], fichier[29] };                                   //on récupère dans un tableau "bbp" les bytes 28 et 29 à convertir pour savoir le nombre de bit par couleur  
            this.nbbitpcolor = Convertir_Endian_To_Int(bbp);                             //on initialise l'attribut nbbitpcolor avec la valeur obtenue à partir de la conversion du tableau précédent, on divise par trois car il y a trois couleurs : rgb
            this.image = ReadImage(fichier);                                             //on initialise l'attribut image avec la valeur obtenue à partir de la méthode qui récupère l'image du fichier de byte
        }
        public MyImage(MyImage Image)
        {
            this.image = Image.Image;                           //chacun des attributs de l'image pris en paramètre est associé au bon attribut de la classe
            this.height = Image.Height;
            this.width = Image.Width;
            this.typeimage = Image.Typeimage;
            this.taillefichier = Image.Taillefichier;
            this.tailleoffset = Image.Tailleoffset;
            this.nbbitpcolor = Image.Nbbitpcolor;
        }
        public MyImage(int height, int width, Pixel[,] matrice)
        {
            this.height = height;                                //prend la largeur en paramètre
            this.width = width;                                  //prend la hauteur en paramètre
            this.typeimage = "bmp";                              //met le type d'image à bitmap
            this.taillefichier = width * height * 24;             //calcule la taille du fichier à partir des dimensions données
            this.tailleoffset = 54;                              //met l'offset à 54 (car image bitmap)
            this.nbbitpcolor = 24;                               //met le nombre de bit par couleur à 24
            this.image = matrice;                                //utilise la matrice de pixel donnée comme image
        }
        #endregion

        #region méthodes principales
        /// <summary>
        /// convertit un tableau de bytes en entier selon little endian
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int j = 0;                                       //variable utilisée pour les puissances de 256
            int taillefichier = 0;                           //variable pour stocker la valeur à retourner (int)
            if (tab != null && tab.Length != 0)              //teste si le tableau donné est bien non nul et non vide
            {
                for (int i = 0; i < tab.Length; i++)    //on part de la dernière case qui contient le bit 
                {
                    taillefichier = taillefichier + tab[i] * Convert.ToInt32(Math.Pow(256, j));  //on ajoute chaque fois 256 à la puissance correspondante fois le nombre contenu dans la case
                    j++;                                    //puis on augmente j pour que la puissance suivante soit plus grande
                }
            }
            else                                            //sinon on renvoie un nombre nul
            {
                taillefichier = 0;
            }
            return taillefichier;                          //on renvoie l'entier obtenu
        }


        /// <summary>
        /// convertit un entier en tableau de byte selon little endian
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] retour = new byte[4];                        //on renvoie un tableau de 4 bytes à la fin
            if (val >= 0)                                       //on vérifie que la valeur soit bien positive
            {
                if (val < (256 * 256 * 256))                    //cas où le plus grand chiffre sera nul
                {
                    if (val < (256 * 256))                      //cas où les deux plus grands chiffres sont nuls
                    {
                        if (val < 256)                          //cas où seul le plus petit chiffre est non nul
                        {
                            retour[3] = 0;                       //cas ou la valeur est inférieure à 256 donc que tous est nul sauf la première case
                            retour[2] = 0;
                            retour[1] = 0;
                            retour[0] = Convert.ToByte(val);
                        }
                        else
                        {
                            retour[3] = 0;                       //cas où la valeur est <256^2 et >=256, on utilise donc deux des cases du tableau
                            retour[2] = 0;
                            int k1 = val / 256;                 //récupère l'entier restant suite à la division par 256 (arrondi sans le reste)
                            retour[1] = Convert.ToByte(k1);     //on le met dans la première case à remplir
                            int reste = val - 256 * k1;         //on retire ce qu'on a déjà rempli au nombre afin de déterminer la valeur du reste pour remplir la dernière case
                            retour[0] = Convert.ToByte(reste);
                        }
                    }
                    else
                    {
                        retour[3] = 0;                           //cas où la valeur est <256^3 et >=256^2, on utilise 3 des cases du tableau
                        int c1 = val / (256 * 256);
                        retour[2] = Convert.ToByte(c1);          //même méthode que ci-dessus mais avec plus de cases
                        int reste = val - 256 * 256 * c1;
                        int c2 = reste / 256;
                        retour[1] = Convert.ToByte(c2);
                        int reste2 = reste - 256 * c2;
                        retour[0] = Convert.ToByte(reste2);
                    }
                }
                else
                {
                    if (val / (256 * 256 * 256) < 255)
                    {
                        int constante1 = val / (256 * 256 * 256);            //cas où la valeur est >=256^3 et <256^4
                        retour[3] = Convert.ToByte(constante1);              //donne la valeur pour la case 256^3 et la met dans la derniere case du tableau
                        int reste = val - 256 * 256 * 256 * constante1;
                        int constante2 = reste / (256 * 256);
                        retour[2] = Convert.ToByte(constante2);              //même méthode que ci-dessus avec toutes les cases à remplir
                        int reste2 = reste - 256 * 256 * constante2;
                        int constante3 = reste2 / 256;
                        retour[1] = Convert.ToByte(constante3);
                        int reste3 = reste2 - 256 * constante3;
                        retour[0] = Convert.ToByte(reste3);
                    }
                    else
                    {
                        retour = null;
                    }
                }
            }
            else
            {
                retour = null;
            }
            return retour;
        }


        /// <summary>
        /// renvoie un string représentant l'image par ses valeurs de pixels
        /// </summary>
        /// <returns></returns>
        public string Tostring()
        {
            string retour = "";                                          //initialise le string à retourner
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)                     //parcoure la matrice de pixel
                {
                    retour += (this.image[i, j].Tostring() + " ");       //retourne chaque valeur des pixels de la matrice image grâce à la méthode tostring de la classe pixel
                    retour += " ";                                       //les sépare d'un espace entre chaque RGB
                }
                retour += '\n';                                          //retour à la ligne à la fin d'une ligne de la matrice
            }
            return retour;
        }


        /// <summary>
        /// prend le fichier de l'image en tableau de byte et renvoie le tableau de pixel de l'image 
        /// </summary>
        /// <param name="fichier"></param>
        /// <returns></returns>
        public Pixel[,] ReadImage(byte[] fichier)
        {
            Pixel[,] mattemppixel = new Pixel[this.height, this.width];  //taille image a partir des autres attributs   
            int complete = 0;
            if ((width * nbbitpcolor / 8) % 4 != 0)                         //calcul du complete pour savoir combien de 0 ont été ajouté si width pas multiple de 4
            {
                int d = (width * nbbitpcolor / 8) / 4;                      //calcule du diviseur (entier) afin de ne pouvoir garder que le reste par la suite
                int r = (width * nbbitpcolor / 8) - 4 * d;                  //soustraction du diviseur fois 4 pour ne garder que le reste compris entre 0 et 3
                complete = 4 - r;                                           //opération pour savoir combien de 0 ont été ajouté afin de compléter les lignes pour être multiple de 4
            }
            int parcours = 54;                                              //variable qui permet de savoir à quelle case du fichier on en est (commence à l'offset)
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)                             //parcours la matrice a remplir
                {
                    Pixel p = new Pixel(fichier[parcours], fichier[parcours + 1], fichier[parcours + 2]);     //créé un nouveau pixel à partir de la lecture du fichier : 3 cases pour créer un pixel R,G,B
                    parcours += 3;                                          //augmente la variable d'indice de 3 pour passer au pixel suivant
                    if (complete != 0 && j == width - 1)                    //si on atteint le bout de la ligne et qu'il y a eu un ajout de 0 pour compléter
                    {
                        parcours += complete;                               //on saute les cases des 0 inutiles
                    }
                    mattemppixel[i, j] = p;                                 //on définit la case de la matrice de pixel avec le pixel créé précedemment
                }
            }
            return mattemppixel;
        }


        /// <summary>
        /// enregistre l'image considérée à partir des attributs et d'un nom de fichier
        /// </summary>
        /// <param name="filename"></param>
        public void From_Image_To_File(string filename)
        {
            int complete = 0;
            if ((image.GetLength(1) * nbbitpcolor / 8) % 4 != 0)                         //calcul du complete pour savoir combien de 0 ont été ajouté si pas multiple de 4
            {
                int d = (image.GetLength(1) * nbbitpcolor / 8) / 4;                      //même méthode que ReadImage pour récupérer le reste et ainsi savoir combien de 0 à ajouter
                int r = (image.GetLength(1) * nbbitpcolor / 8) - 4 * d;
                complete = 4 - r;
            }

            byte[] taille = Convertir_Int_To_Endian(54 + this.height * (this.width + complete) * 3);  //création des tableaux de bytes servant à remplir tout le header
            byte[] ofst = Convertir_Int_To_Endian(this.tailleoffset);                         //à l'aide de la fonction de conversion en little endian
            byte[] sizeheader = Convertir_Int_To_Endian(40);
            byte[] wid = Convertir_Int_To_Endian(this.width);
            byte[] hei = Convertir_Int_To_Endian(this.height);
            byte[] colorplane = Convertir_Int_To_Endian(1);
            byte[] bitpercolor = Convertir_Int_To_Endian(24);
            byte[] imtaille = Convertir_Int_To_Endian(this.width * this.height / 3);
            byte[] tabbyte = new byte[54 + this.height * (this.width + complete) * 3];      //création du tableau de byte qui va être utiliser pour enregistrer le fichier (taille prend en compte header et image)
            tabbyte[0] = 66;                                                                //enregistrement au format bitmap 
            tabbyte[1] = 77;                                                                //donc remplissage de chacune des cases selon ce format
            for (int i = 2; i < 6; i++) { tabbyte[i] = taille[i - 2]; }
            for (int j = 6; j < 10; j++) { tabbyte[j] = 0; }                                //byte[] adressecreateur = { 0, 0, 0, 0 };
            tabbyte[10] = ofst[0];
            tabbyte[11] = ofst[1];
            tabbyte[12] = ofst[2];
            tabbyte[13] = ofst[3];
            tabbyte[14] = sizeheader[0];
            tabbyte[15] = sizeheader[1];
            tabbyte[16] = sizeheader[2];
            tabbyte[17] = sizeheader[3];
            tabbyte[18] = wid[0];
            tabbyte[19] = wid[1];
            tabbyte[20] = wid[2];
            tabbyte[21] = wid[3];
            tabbyte[22] = hei[0];
            tabbyte[23] = hei[1];
            tabbyte[24] = hei[2];
            tabbyte[25] = hei[3];
            tabbyte[26] = colorplane[0];
            tabbyte[27] = colorplane[1];
            tabbyte[28] = bitpercolor[0];
            tabbyte[29] = bitpercolor[1];
            for (int j = 30; j < 34; j++) { tabbyte[j] = 0; }                              //byte[] compression = { 0, 0, 0, 0 };
            tabbyte[34] = imtaille[0];                                                     //pas nécessaire : on pourrait simplement mettre quatre 0 à la place
            tabbyte[35] = imtaille[1];
            tabbyte[36] = imtaille[2];
            tabbyte[37] = imtaille[3];
            for (int j = 38; j < 54; j++) { tabbyte[j] = 0; }
            int z = 54;
            for (int k = 0; k < this.height; k++)
            {
                for (int l = 0; l < this.width; l++)
                {
                    tabbyte[z] = this.image[k, l].R;                                //remplissage de la fin du fichier par tousles pixels de l'image
                    tabbyte[z + 1] = this.image[k, l].G;
                    tabbyte[z + 2] = this.image[k, l].B;
                    z += 3;
                }
                for (int b = 0; b < complete; b++)
                {
                    tabbyte[z] = 0;                                                //ajout des 0 pour compléter si nécessaire (en cas de width non multiple de 4)
                    z++;
                }
            }
            File.WriteAllBytes("./" + filename + ".bmp", tabbyte);                 //enregistrement du tableau de byte en fichier bitmap avec le nom choisi en paramètre
        }


        /// <summary>
        /// Convertit l'entier en binaire et renvoie un tableau de nbbit cases
        /// </summary>
        /// <param name="entree">nombre entier donné à convertir</param>
        /// <param name="nbbit">nombre de bit souhaité dans le tableau en sortie</param>
        /// <returns></returns>
        public int[] ConversionEntierToBinaire(int entree, int nbbit)
        {
            int[] retour = new int[nbbit];
            int var = 0;
            for (int i = nbbit - 1; i >= 0; i--)           //comme la plus petite puissance de 2 est à droite, on commence à la dernière case du tableau pour bien remplir dans l'ordre
            {
                var = entree % 2;                          //récupère le reste de la division par 2 (donc 0 ou 1)
                retour[i] = var;                           //met le reste dans la dernière case
                entree = entree / 2;                       //divise par deux l'entier (arrondi car c'est un entier)
            }
            return retour;
        }


        /// <summary>
        /// Convertit le tableau donné en binaire en entrée en un entier
        /// </summary>
        /// <param name="tabbyte">tableau binaire à convertir en entier</param>
        /// <returns></returns>
        public int ConversionBinaireToEntier(int[] tabbyte)
        {
            int retour = 0;                                                                     //initialisation de l'entier quon retourne à la fin
            for (int i = tabbyte.Length - 1; i >= 0; i--)                                       //on commence au bout du tableau pour commencer par les plus petites puissances de 2
            {
                retour += Convert.ToInt32(tabbyte[i] * Math.Pow(2, (tabbyte.Length - 1 - i)));  //ajout au résultat de la valeur de la case (0 ou1) *2^tabbyte.Lenght-1 pour que la puissance commence à 0 et augmente au fur et à mesure
            }
            return retour;
        }
        #endregion 

        #region filtres

        /// <summary>
        /// applique un filtre noir et blanc à l'image
        /// </summary>
        public void Noir_et_blanc()
        {
            int moyenne = 0;
            Pixel[,] noiretblanc = new Pixel[this.height, this.width];
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)               //on parcoure la matrice de pixel de l'image
                {
                    moyenne = (Convert.ToInt32(this.image[i, j].R) + Convert.ToInt32(this.image[i, j].G) + Convert.ToInt32(this.image[i, j].B)) / 3;   //on fait la moyenne de R G et B pour chaque pixel
                    if (moyenne < 127)                 //si cette moyenne est entre 0 et 126 alors on met le pixel en noir
                    {
                        Pixel p = new Pixel(0, 0, 0);
                        noiretblanc[i, j] = p;
                    }
                    else
                    {
                        Pixel p = new Pixel(255, 255, 255);  //sinon on met le pixel blanc
                        noiretblanc[i, j] = p;
                    }
                }
            }
            this.image = noiretblanc;
        }


        /// <summary>
        /// applique un filtre nuance de gris à l'image
        /// </summary>
        public void Nuances_De_Gris()
        {
            int moyenne = 0;
            Pixel[,] nuancesdegris = new Pixel[this.height, this.width];
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    moyenne = (Convert.ToInt32(this.image[i, j].R) + Convert.ToInt32(this.image[i, j].G) + Convert.ToInt32(this.image[i, j].B)) / 3; //moyenne de R G et B pour un pixel
                    Pixel p = new Pixel(Convert.ToByte(moyenne), Convert.ToByte(moyenne), Convert.ToByte(moyenne)); //on donne cette valeur de moyenne à R G et B pour obtenir la nuance de gris
                    nuancesdegris[i, j] = p;
                }
            }
            this.image = nuancesdegris;
        }


        /// <summary>
        /// applique un filtre négatif à l'image
        /// </summary>
        public void Negatif()
        {
            Pixel[,] negatif = new Pixel[this.height, this.width];
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {

                    Pixel p = new Pixel(Convert.ToByte(255 - this.image[i, j].R), Convert.ToByte(255 - this.image[i, j].G), Convert.ToByte(255 - this.image[i, j].B));
                    negatif[i, j] = p;
                }
            }
            this.image = negatif;
        }


        /// <summary>
        /// applique un filtre effet miroir à l'image
        /// </summary>
        public void Effet_Miroir()
        {
            Pixel[,] effetmiroir = new Pixel[this.height, this.width];      //pour effet miroir on met ce qui est à gauche à droite et ce qui est à droite à gauche
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)                       //on parcoure la matrice de pixels
                {
                    effetmiroir[i, j] = this.image[i, width - 1 - j];      //on renvoie le pixel qui à la même hauteur mais qui a pour largeur width-1-j pour renvoyer le pixel à l'opposé en term de gauche droite
                }
            }
            this.image = effetmiroir;
        }


        /// <summary>
        /// rétrécit l'image ou l'agrandit suivant le coefficient fourni
        /// </summary>
        /// <param name="coef"></param>
        public void Agrandir_Retrecir(double coef)
        {
            int newheight = Convert.ToInt32(coef * this.height);  //initialisation avec la hauteur initiale fois le coefficient souhaité donné en paramètre
            int newwidth = Convert.ToInt32(coef * this.width);    //initialisation avec la largeur initiale fois le coefficient souhaité donné en paramètre

            Pixel[,] agrandirretrecir = new Pixel[newheight, newwidth];   //on créé la matrice de pixel à renvoyer avec les nouvelles dimensions
            Pixel p = new Pixel(0, 0, 0);
            for (int i = 0; i < newheight; i++)
            {
                for (int j = 0; j < newwidth; j++)                    //on parcoure la matrice de pixels
                {
                    int div = Convert.ToInt32(i / coef);                       //pour chaque case on calcule à quelle ancienne case cela correspond
                    int div2 = Convert.ToInt32(j / coef);
                    if (div >= this.height) { div = this.height - 1; }         //on vérifie que l'on ne dépasse pas le bornes à cause des arrondis, sinon on change leur valeur
                    if (div2 >= this.width) { div2 = this.width - 1; }
                    agrandirretrecir[i, j] = this.image[div, div2];             //on donne au pixel i,j de la nouvelle matrice, la valeur du pixel div,div2 de l'image initiale
                }
            }
            this.image = agrandirretrecir;                                     //on met à jour les attributs obtenus pour cette image agrandie/rétrécie 
            this.height = newheight;
            this.width = newwidth;
            this.taillefichier = this.width * this.height * 3;
        }


        /// <summary>
        /// tourne l'image suivant l'angle donné en degré
        /// </summary>
        /// <param name="angle"></param>
        public void Rotation(int angle)
        {
            int length = Convert.ToInt32(Math.Sqrt(width * width + height * height));      //grande diagonale pour être sur que l'image rentre quel que soit le degré de rotation
            Pixel[,] avantrotation = new Pixel[length, length];
            //matrice initiale pas tournée
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    Pixel p = new Pixel(255, 255, 255);                              //on remplit de blanc comme ça toutes les cases seront remplis
                    avantrotation[i, j] = p;
                }
            }
            int xdebut = (length - height) / 2;
            int ydebut = (length - width) / 2;                                    //pour tourner autour de son centre on commence au milieu
            for (int k = 0; k < height; k++)
            {
                for (int l = 0; l < width; l++)
                {
                    avantrotation[k + xdebut, l + ydebut] = image[k, l];              //on met la matrice initiale au milieu de l'image blanche pour avoir une matrice initiale pour tourner
                }
            }
            //nouvelle matrice
            Pixel[,] rotation = new Pixel[length, length];                     //on créé la matrice qui va avoit l'image tournée
            int x = 0;
            int y = 0;
            int xnew = 0;
            int ynew = 0;
            int xreconvert;
            int yreconvert;
            for (int o = 0; o < length; o++)
            {
                for (int p = 0; p < length; p++)
                {
                    Pixel h = new Pixel(255, 255, 255);               //on la rempli de blanc pour que toutes les cases soient remplies
                    rotation[o, p] = h;
                }
            }
            //conversion angle degré en radian
            double rad = (angle * Math.PI) / 180;
            for (int a = 0; a < length; a++)
            {
                for (int b = 0; b < length; b++)
                {
                    ///conversion nouveau repère des coordonnées
                    xnew = -length / 2 + a;
                    ynew = -b + length / 2;
                    x = Convert.ToInt32(xnew * Math.Cos(rad) + ynew * Math.Sin(rad));
                    y = Convert.ToInt32(-xnew * Math.Sin(rad) + ynew * Math.Cos(rad));     //on utilise la conversion en coordonnées polaires pour trouver les nouvelles coordonnées
                    //reconversion
                    xreconvert = x + length / 2;
                    yreconvert = -y + length / 2;
                    if (xreconvert > 0 && xreconvert < length && yreconvert > 0 && yreconvert < length) //si les valeurs obtenues sont bien dans les bornes, on peut remplir le pixel
                    {
                        rotation[a, b] = avantrotation[xreconvert, yreconvert];                       //le pixel de la nouvelle matrice est rempli avec le pixel de l'ancienne dont les coordonnées ont été calculé
                    }
                }
            }
            this.image = rotation;                                     //on met à jour les attributs de la matrice tournée
            this.height = length;
            this.width = length;
            this.taillefichier = this.width * this.height * 3;
        }

        #endregion

        #region filtres matrice de convolution 

        //pour chacune des fonctions qui suivent, on définit simplement la matrice et le diviseur(utile pour le flou) à appliquer à la fonction matrice de convolution 
        //afin d'obtenir la nouvelle matrice de pixels avec les changements souhaités

        /// <summary>
        /// applique un filtre flou à l'image
        /// </summary>
        public void Flou()
        {
            double[,] convolutionflou = new double[5, 5] { { 1, 4, 6, 4, 1 }, { 4, 16, 24, 16, 4 }, { 6, 24, 36, 24, 6 }, { 4, 16, 24, 16, 4 }, { 1, 4, 6, 4, 1 } };
            //{{1,1,1},{1,1,1 },{1,1,1}}
            int div = 256;
            this.image = MatriceConvolution(convolutionflou, div);
        }


        /// <summary>
        /// applique un filtre de détection de contour
        /// </summary>
        public void DetectionContour()
        {
            double[,] convolutiondeteccontour = new double[3, 3] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            int div = 1;
            this.image = MatriceConvolution(convolutiondeteccontour, div);
        }


        /// <summary>
        /// applique un filtre de repoussage à l'image
        /// </summary>
        public void Repoussage()
        {
            double[,] convolutionrepoussage = new double[3, 3] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } }; //{ { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } }
            int div = 1;
            this.image = MatriceConvolution(convolutionrepoussage, div);
        }


        /// <summary>
        /// applique un filtre de renforcement des bord à l'image
        /// </summary>
        public void RenforcementBord()
        {
            double[,] convolutionrenforcementbord = new double[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } }; //{ { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } }
            int div = 1;
            this.image = MatriceConvolution(convolutionrenforcementbord, div);
        }


        /// <summary>
        /// fonction qui applique le filtre grâce à la matrice donnée et un coefficient de division
        /// </summary>
        /// <param name="matconv"></param>
        /// <param name="div"></param>
        /// <returns></returns>
        public Pixel[,] MatriceConvolution(double[,] matconv, int div)
        {
            Pixel[,] retour = new Pixel[height, width];
            double calcul1 = 0;
            double calcul2 = 0;
            double calcul3 = 0;
            int abs = 0;
            int ordo = 0;
            int bornei = (matconv.GetLength(0) - 1) / 2;       //permet de savoir de combien de pixels autour du pixel considéré on va utiliser afin de calculer la nouvelle valeur de ce pixel
            int bornej = (matconv.GetLength(1) - 1) / 2;
            int g = 0;
            int h = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int a = i - bornei; a <= i + bornei; a++)
                    {
                        for (int b = j - bornej; b <= j + bornej; b++)      //on parcourt la matrice entre un certain nombre avant le pixel actuel et un certain nombre après
                        {
                            if (a < 0)                                     //si le pixel autour considéré a un indice de hauteur inférieur à 0, va chercher le pixel symétrique par rapport au centre 
                            {                                              //donc vers la fin de la matrice afin de faire comme si les bords de la matrice était en cercle
                                abs = height + a;
                                if (b < 0)                                //on fait la même chose avec la largeur
                                {
                                    ordo = width + b;
                                }
                                else
                                {
                                    if (b >= width)                       //de la même manière si on est trop à la fin on prend un équivalent au début
                                    {
                                        ordo = b - width;
                                    }
                                    else
                                    {
                                        ordo = b;
                                    }
                                }
                            }                                        //on traite ainsi tous les cas possible afin d'avoir des indices bien dans les bornes
                            else
                            {
                                if (a >= height)
                                {
                                    abs = a - height;
                                    if (b < 0)
                                    {
                                        ordo = width + b;
                                    }
                                    else
                                    {
                                        if (b >= width)
                                        {
                                            ordo = b - width;
                                        }
                                        else
                                        {
                                            ordo = b;
                                        }
                                    }
                                }
                                else
                                {
                                    abs = a;
                                    if (b < 0)
                                    {
                                        ordo = width + b;
                                    }
                                    else
                                    {
                                        if (b >= width)
                                        {
                                            ordo = b - width;
                                        }
                                        else
                                        {
                                            ordo = b;
                                        }
                                    }
                                }
                            }

                            calcul1 += this.image[abs, ordo].R * matconv[g, h];     //une fois les indices bons on ajoute pour chacune des trois variables la valeur de l'entier R, G ou B 
                            calcul2 += this.image[abs, ordo].G * matconv[g, h];     //présent à cet indice
                            calcul3 += this.image[abs, ordo].B * matconv[g, h];
                            h++;
                        }
                        g++;
                        h = 0;
                    }
                    g = 0;
                    h = 0;
                    calcul1 = calcul1 / div;                                   //on divise ces entiers par div (utile pour le flou)
                    calcul2 = calcul2 / div;
                    calcul3 = calcul3 / div;
                    if (calcul1 > 255) calcul1 = 255;            //on vérifie que les nombres obtenus sont bien entre 0 et 255 sinon on les modifie
                    if (calcul2 > 255) calcul2 = 255;
                    if (calcul3 > 255) calcul3 = 255;
                    if (calcul1 < 0) calcul1 = 0;
                    if (calcul2 < 0) calcul2 = 0;
                    if (calcul3 < 0) calcul3 = 0;
                    Pixel p = new Pixel(Convert.ToByte(calcul1), Convert.ToByte(calcul2), Convert.ToByte(calcul3)); //on créé le pixel à partir des valeurs obtenues
                    calcul1 = 0;
                    calcul2 = 0;
                    calcul3 = 0;       //on remet tous à zéro pour le tour suivant
                    retour[i, j] = p; //on définit le pixel de la matrice avec p le pixel créé avec les calculs
                }
            }
            return retour;
        }

        #endregion

        #region histogrammes

        /// <summary>
        /// renvoie une matrice de pixels de l'histogramme rouge
        /// </summary>
        /// <returns></returns>
        public Pixel[,] HistrogrammeRouge()
        {
            int[] tab = new int[256];                    //on créé un tableau ayant 256 cases pour des valeurs entre 0 et 255
            for (int a = 0; a < 256; a++)
            {
                tab[a] = 0;                              //on remplit le tableau de 0 par défaut
            }
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    tab[this.image[i, j].R]++;            //on parcourt toute l'image et on augmente de 1 la case du tab correspondant à la valeur du pixel rouge étudié
                }                                         //ainsi notre tableau sera rempli du nombre de fois où le pixel rouge est à telle valeur (valeur correspondant à l'indice du tableau)
            }
            int max = tab[0];                            //on initialise le max à la première valeur au début
            for (int b = 1; b < 256; b++)
            {
                if (tab[b] > max) max = tab[b];          //puis si on trouve une valeur plus importante on le change
            }
            Pixel[,] retour = new Pixel[100, 256];       //on créé la matrice à retourner qui fait 256 de largeur pour avoir chaque fois 0 à 255 en valeurs et 100 pour 100%

            for (int i = 0; i < retour.GetLength(0); i++)
            {
                for (int j = 0; j < retour.GetLength(1); j++)
                {
                    Pixel p = new Pixel(0, 0, 0);        //on  remplit de noir la matrice afin que toutes les cases soient remplies
                    retour[i, j] = p;
                }
            }
            for (int z = 0; z < retour.GetLength(1); z++)
            {
                for (int x = retour.GetLength(0) - tab[z] * 100 / max; x < retour.GetLength(0); x++) //pour toutes les cases qui sont inférieurs au pourcentage pour cette valeur
                {
                    Pixel p = new Pixel(0, 0, 255);
                    retour[retour.GetLength(0) - 1 - x, z] = p;           //on remplit en rouge la case
                }
            }
            return retour;
        }


        /// <summary>
        /// renvoie une matrice de pixels de l'histogramme vert
        /// </summary>
        /// <returns></returns>
        public Pixel[,] HistrogrammeVert()
        {
            int[] tab = new int[256];                               //même méthode que histogramme rouge
            for (int a = 0; a < 256; a++)
            {
                tab[a] = 0;
            }
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    tab[this.image[i, j].G]++;
                }
            }
            int max = tab[0];
            for (int b = 1; b < 256; b++)
            {
                if (tab[b] > max) max = tab[b];
            }
            Pixel[,] retour = new Pixel[100, 256];

            for (int i = 0; i < retour.GetLength(0); i++)
            {
                for (int j = 0; j < retour.GetLength(1); j++)
                {
                    Pixel p = new Pixel(0, 0, 0);
                    retour[i, j] = p;
                }
            }
            for (int z = 0; z < retour.GetLength(1); z++)
            {
                for (int x = retour.GetLength(0) - tab[z] * 100 / max; x < retour.GetLength(0); x++)
                {
                    Pixel p = new Pixel(0, 255, 0);
                    retour[retour.GetLength(0) - 1 - x, z] = p;
                }
            }
            return retour;
        }


        /// <summary>
        /// renvoie une matrice de pixels de l'histogramme bleu
        /// </summary>
        /// <returns></returns>
        public Pixel[,] HistrogrammeBleu()
        {
            int[] tab = new int[256];                                  //même méthode que histogramme rouge
            for (int a = 0; a < 256; a++)
            {
                tab[a] = 0;
            }
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    tab[this.image[i, j].B]++;
                }
            }
            int max = tab[0];
            for (int b = 1; b < 256; b++)
            {
                if (tab[b] > max) max = tab[b];
            }
            Pixel[,] retour = new Pixel[100, 256];

            for (int i = 0; i < retour.GetLength(0); i++)
            {
                for (int j = 0; j < retour.GetLength(1); j++)
                {
                    Pixel p = new Pixel(0, 0, 0);
                    retour[i, j] = p;
                }
            }
            for (int z = 0; z < retour.GetLength(1); z++)
            {
                for (int x = retour.GetLength(0) - tab[z] * 100 / max; x < retour.GetLength(0); x++)
                {
                    Pixel p = new Pixel(255, 0, 0);
                    retour[retour.GetLength(0) - 1 - x, z] = p;
                }
            }
            return retour;
        }


        /// <summary>
        /// renvoie une matrice de pixels de l'histogramme nuances de gris
        /// </summary>
        /// <returns></returns>
        public Pixel[,] HistrogrammeNuanceGris()
        {
            int[] tab = new int[256];                           //même méthode que précedemment sauf qu'on fait la moyenne au lieu de considéré que R G ou B
            for (int a = 0; a < 256; a++)
            {
                tab[a] = 0;
            }
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    tab[(this.image[i, j].R + this.image[i, j].G + this.image[i, j].B) / 3]++;
                }
            }
            int max = tab[0];
            for (int b = 1; b < 256; b++)
            {
                if (tab[b] > max) max = tab[b];
            }
            Pixel[,] retour = new Pixel[100, 256];

            for (int i = 0; i < retour.GetLength(0); i++)
            {
                for (int j = 0; j < retour.GetLength(1); j++)
                {
                    Pixel p = new Pixel(0, 0, 0);
                    retour[i, j] = p;
                }
            }
            for (int z = 0; z < retour.GetLength(1); z++)
            {
                for (int x = retour.GetLength(0) - tab[z] * 100 / max; x < retour.GetLength(0); x++)
                {
                    Pixel p = new Pixel(125, 125, 125);
                    retour[retour.GetLength(0) - 1 - x, z] = p;
                }
            }
            return retour;
        }

        #endregion

        #region fractale

        /// <summary>
        /// renvoie une matrice de pixels de la fractale
        /// </summary>
        /// <returns></returns>
        public Pixel[,] Fractale() //prb parce qu'on pars d'une image existante
        {
            int taille1 = 3000;
            int taille2 = 3000;
            Pixel[,] retour = new Pixel[taille1, taille2];
            //on utilise la méthode de l'ensemble de Mandelbrot
            int iterationmax = 50;
            double zpartier = 0;
            double zpartieim = 0;
            int i = 0;
            double cpartier = 0;
            double cpartieim = 0;
            double temp = 0;
            for (int x = 0; x < taille1; x++)
            {
                for (int y = 0; y < taille2; y++)
                {
                    cpartier = (y - (taille2) / 2.0) / (taille2 / 3);
                    cpartieim = (x - (taille1) / 2.0) / (taille1 / 3);
                    zpartier = 0;
                    zpartieim = 0;
                    i = 0;
                    //condition du while correspond au module<2 donc pour éviter d'avoir une racine on met au carre :
                    while (zpartier * zpartier + zpartieim * zpartieim < 4 && i < iterationmax)
                    {
                        /* temp = (zpartier * zpartier)-(zpartieim * zpartieim)+cpartier;
                         //z=z^2+c donc en ecriture complexe :
                         zpartieim = 2 * zpartieim * zpartieim + cpartieim;
                         zpartier = temp;
                         i++;*/
                        temp = zpartier;
                        zpartier = (zpartier * zpartier) - (zpartieim * zpartieim) + cpartier;
                        zpartieim = 2 * zpartieim * temp + cpartieim;
                        i++;
                    }
                    if (i == iterationmax)
                    {
                        retour[x, y] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        retour[x, y] = new Pixel((byte)(i * 255 / iterationmax), 0, (byte)(i * 255 / iterationmax));
                    }
                }
            }
            return retour;
        }

        #endregion

        #region cacher une image dans une autre

        /// <summary>
        /// renvoie l'image avec l'image cachée en matrice de pixels
        /// </summary>
        /// <param name="imageforte"></param>
        /// <param name="imagefaible"></param>
        /// <returns></returns> 
        public Pixel[,] CacherImageDansImage(Pixel[,] imageforte, Pixel[,] imagefaible)
        {
            Pixel[,] retour = new Pixel[imageforte.GetLength(0), imageforte.GetLength(1)];
            if (imageforte.GetLength(0) == imagefaible.GetLength(0) && imageforte.GetLength(1) == imagefaible.GetLength(1))  //on vérifie que les deux images ont la même taille pour que ça rentre bien
            {
                for (int i = 0; i < imageforte.GetLength(0); i++)
                {
                    for (int j = 0; j < imageforte.GetLength(1); j++)
                    {
                        retour[i, j] = imageforte[i, j];                      //on met d'abord l'image forte dans toutes les cases
                    }
                }
                for (int i = 0; i < imagefaible.GetLength(0); i++)
                {
                    for (int j = 0; j < imagefaible.GetLength(1); j++)       //puis on change les pixels ou on doit cacher aussi l'image faible derriere
                    {
                        //il faut d'abord récupérer les int des pixels des deux images, les convertir en binaire, les concatener puis les reconvertir en int pour pixel
                        byte p1 = (byte)(ConversionBinaireToEntier(Cachebyte(ConversionEntierToBinaire((int)(imageforte[i, j].R), 8), ConversionEntierToBinaire((int)(imagefaible[i, j].R), 8))));
                        byte p2 = (byte)(ConversionBinaireToEntier(Cachebyte(ConversionEntierToBinaire((int)(imageforte[i, j].G), 8), ConversionEntierToBinaire((int)(imagefaible[i, j].G), 8))));
                        byte p3 = (byte)(ConversionBinaireToEntier(Cachebyte(ConversionEntierToBinaire((int)(imageforte[i, j].B), 8), ConversionEntierToBinaire((int)(imagefaible[i, j].B), 8))));
                        Pixel p = new Pixel(p1, p2, p3);
                        retour[i, j] = p;
                    }
                }
            }
            return retour;
        }


        /// <summary>
        /// renvoie une matrice de pixels de l'image principale récupérée
        /// </summary>
        /// <param name="imagemixte"></param>
        /// <returns></returns>
        public Pixel[,] DecacherImageForte(Pixel[,] imagemixte)
        {
            Pixel[,] retour = new Pixel[imagemixte.GetLength(0), imagemixte.GetLength(1)];
            for (int i = 0; i < imagemixte.GetLength(0); i++)
            {
                for (int j = 0; j < imagemixte.GetLength(1); j++)  //pour chaque pixel on utilise les fonctions de conversions pour récupérer les pixels de l'image forte
                {
                    //il faut d'abord récupérer les int des pixels des deux images, les convertir en binaire, les concatener puis les reconvertir en int pour pixel
                    byte p1 = (byte)(ConversionBinaireToEntier(Decachebytefort(ConversionEntierToBinaire((int)(imagemixte[i, j].R), 8))));
                    byte p2 = (byte)(ConversionBinaireToEntier(Decachebytefort(ConversionEntierToBinaire((int)(imagemixte[i, j].G), 8))));
                    byte p3 = (byte)(ConversionBinaireToEntier(Decachebytefort(ConversionEntierToBinaire((int)(imagemixte[i, j].B), 8))));
                    Pixel p = new Pixel(p1, p2, p3);
                    retour[i, j] = p;
                }
            }
            return retour;
        }


        /// <summary>
        /// renvoie une matrice de pixels de l'image cachée récupérée
        /// </summary>
        /// <param name="imagemixte"></param>
        /// <returns></returns>
        public Pixel[,] DecacherImageFaible(Pixel[,] imagemixte)
        {
            Pixel[,] retour = new Pixel[imagemixte.GetLength(0), imagemixte.GetLength(1)];
            for (int i = 0; i < imagemixte.GetLength(0); i++)
            {
                for (int j = 0; j < imagemixte.GetLength(1); j++)    //pour chaque pixel on utilise les fonctions de conversion pour récuperer le pixel de l'image faible cachée
                {
                    //il faut d'abord récupérer les int des pixels des deux images, les convertir en binaire, les concatener puis les reconvertir en int pour pixel
                    byte p1 = (byte)(ConversionBinaireToEntier(Decachebytefaible(ConversionEntierToBinaire((int)(imagemixte[i, j].R), 8))));
                    byte p2 = (byte)(ConversionBinaireToEntier(Decachebytefaible(ConversionEntierToBinaire((int)(imagemixte[i, j].G), 8))));
                    byte p3 = (byte)(ConversionBinaireToEntier(Decachebytefaible(ConversionEntierToBinaire((int)(imagemixte[i, j].B), 8))));
                    Pixel p = new Pixel(p1, p2, p3);
                    retour[i, j] = p;
                }
            }
            return retour;
        }


        /// <summary>
        /// créé une matrice de pixel de la taille de l'image principale et met l'autre image dedans
        /// </summary>
        /// <param name="image"></param>
        /// <param name="heightautreimage"></param>
        /// <param name="widthautreimage"></param>
        /// <returns></returns>
        public Pixel[,] FondBlanc(Pixel[,] image, int heightautreimage, int widthautreimage)
        {
            Pixel[,] retour = new Pixel[heightautreimage, widthautreimage];
            for (int i = 0; i < heightautreimage; i++)
            {
                for (int j = 0; j < widthautreimage; j++)
                {
                    retour[i, j] = new Pixel(0, 0, 0);           //on remplit l'image de la taille de l'image forte en noir
                }
            }
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    retour[i, j] = image[i, j];               //on met l'image faible dedans comme ça ça a la même taille
                }
            }
            return retour;
        }


        /// <summary>
        /// prend un pixel de l'image principale et de l'autre en tableau d'entier et renvoie le tbaleau d'entier mixant les deux
        /// </summary>
        /// <param name="forte"></param>
        /// <param name="faible"></param>
        /// <returns></returns>
        public int[] Cachebyte(int[] forte, int[] faible)
        {
            int[] retour = new int[8];
            retour[0] = forte[0];         //on met les quatre premiers bytes avec la valeur des quatre premiers bytes de l'image forte
            retour[1] = forte[1];
            retour[2] = forte[2];
            retour[3] = forte[3];
            retour[4] = faible[0];       ////on met les quatre derniers bytes avec la valeur des quatre premiers bytes de l'image faible
            retour[5] = faible[1];
            retour[6] = faible[2];
            retour[7] = faible[3];

            return retour;
        }


        /// <summary>
        /// renvoie le tableau d'entier du pixel de l'image principale
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public int[] Decachebytefort(int[] cache)
        {
            int[] retour = new int[8];
            retour[0] = cache[0];      //les quatre premiers bytes de l'image forte sont les quatre premiers bytes de l'image cachée
            retour[1] = cache[1];
            retour[2] = cache[2];
            retour[3] = cache[3];
            retour[4] = 0;             //on met à 0 les bytes de poids faibles pour compléter
            retour[5] = 0;
            retour[6] = 0;
            retour[7] = 0;
            return retour;
        }


        /// <summary>
        /// renvoie le tableau d'entier du pixel de l'image cachée
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public int[] Decachebytefaible(int[] cache)
        {
            int[] retour = new int[8];
            retour[0] = cache[4];       //les quatre premiers bytes de l'image faible sont les quatre derniers de l'image cachée
            retour[1] = cache[5];
            retour[2] = cache[6];
            retour[3] = cache[7];
            retour[4] = 0;             //les quatre derniers sont mis à 0 car de poids faible pour compléter
            retour[5] = 0;
            retour[6] = 0;
            retour[7] = 0;
            return retour;
        }

        #endregion

        #region génération qrcode

        /// <summary>
        /// encode les premiers caractères nécessaires : alphanumérique et taille chaine
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>
        public string Encodeur(string chaine)
        {
            string retour = "";
            int[] tabtaillebin;
            retour += "0010";            //correspond au code binaire pour alphanumérique
            if (chaine != null)
            {
                int taille = chaine.Length;
                if (taille <= 25 && taille != 0)  //suivant si c'est le qr code 21x21 ou 25x25
                {
                    tabtaillebin = ConversionEntierToBinaire(taille, 9);  //on convertit en 9 bits la taille pour la mettre au début de la chaine binaire qui va remplir le qr code
                    //utilisation version 1 : 21x21
                }
                else
                {
                    if (taille <= 47)
                    {
                        tabtaillebin = ConversionEntierToBinaire(taille, 9); //même conversion de la taille pour le qr code taille 25x25
                        //utilisation version 2 : 25x25
                    }
                    else
                    {
                        tabtaillebin = null;
                    }
                }
                if (tabtaillebin != null)
                {
                    for (int i = 0; i < tabtaillebin.Length; i++)
                    {
                        retour += Convert.ToString(tabtaillebin[i]);        //on convertit le tableau de binaire en string pour l'ajouter au résultat par la suite
                    }
                }
            }
            return retour;
        }


        /// <summary>
        /// renvoie le tableau de pixel du QR code taille 1 : 21x21
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>
        public void QRCode(string chaine, int length)
        {

            Pixel[,] qrcode = new Pixel[length, length];   //initialisation du qr code en fonction de la version de qr code choisi (21 ou 25)

            //création des motif de recherche

            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    qrcode[i, j] = new Pixel(125, 125, 125);             //on remplit tout en gris pour pouvoir différencier des motifs qu'on va ajouter et remplir toutes les cases pour comparer ensuite
                }
            }

            #region motifs carrés

            //------------------------Motif en bas à gauche-------------------------------------------

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {

                    if (i == 0 || i == 6 || j == 0 || j == 6)
                    {
                        qrcode[i, j] = noir;
                    }
                    else
                    {
                        if (i == 1 || i == 5 || j == 1 || j == 5 || i == 7 || j == 7)
                        {
                            qrcode[i, j] = blanc;
                        }
                        else
                        {
                            qrcode[i, j] = noir;
                        }
                    }

                }
            }

            //lignes blanches extérieures
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    if (i == 7 || j == 7)
                    {
                        qrcode[i, j] = blanc;
                    }
                }
            }

            //-----------------------------Motif en haut à gauche---------------------------------------------------------

            for (int i = qrcode.GetLength(0) - 8; i <= qrcode.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == qrcode.GetLength(0) - 7 || i == qrcode.GetLength(0) - 1 || j == 0 || j == 6)
                    {
                        qrcode[i, j] = noir;
                    }
                    else
                    {
                        if (i == qrcode.GetLength(0) - 6 || i == qrcode.GetLength(0) - 2 || i == qrcode.GetLength(0) - 8 || j == 1 || j == 5 || j == 7)
                        {
                            qrcode[i, j] = blanc;
                        }
                        else
                        {
                            qrcode[i, j] = noir;
                        }
                    }

                }
            }
            //lignes blanches extérieures
            for (int i = qrcode.GetLength(0) - 8; i <= qrcode.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    if (i == qrcode.GetLength(0) - 8 || j == 7)
                    {
                        qrcode[i, j] = blanc;
                    }
                }
            }


            //---------------------------------Motif en haut à droite-----------------------------------------------------------------

            for (int i = qrcode.GetLength(0) - 8; i <= qrcode.GetLength(0) - 1; i++)
            {
                for (int j = qrcode.GetLength(1) - 8; j <= qrcode.GetLength(1) - 1; j++)
                {
                    if (i == qrcode.GetLength(0) - 7 || i == qrcode.GetLength(0) - 1 || j == qrcode.GetLength(0) - 7 || j == qrcode.GetLength(0) - 1)
                    {
                        qrcode[i, j] = noir;
                    }
                    else
                    {
                        if (i == qrcode.GetLength(0) - 6 || i == qrcode.GetLength(0) - 2 || i == qrcode.GetLength(0) - 8 || j == qrcode.GetLength(0) - 6 || j == qrcode.GetLength(0) - 2 || j == qrcode.GetLength(0) - 8)
                        {
                            qrcode[i, j] = blanc;
                        }
                        else
                        {
                            qrcode[i, j] = noir;
                        }
                    }

                }
            }
            //lignes blanches extérieures
            for (int i = qrcode.GetLength(0) - 8; i <= qrcode.GetLength(0) - 1; i++)
            {
                for (int j = qrcode.GetLength(1) - 8; j <= qrcode.GetLength(1) - 1; j++)
                {
                    if (i == qrcode.GetLength(0) - 8 || j == qrcode.GetLength(0) - 8)
                    {
                        qrcode[i, j] = blanc;
                    }
                }
            }
            #endregion

            #region lignes pointillées
            //------------------------------------motif de synchronisation vertical------------------------------------------

            for (int i = 8; i <= qrcode.GetLength(0) - 9; i += 2)
            {
                qrcode[i, 6] = noir;
                qrcode[i + 1, 6] = blanc;
            }

            //------------------------------------motif de synchronisation horizontale---------------------------------------------

            for (int j = 8; j <= qrcode.GetLength(0) - 9; j += 2)
            {
                qrcode[qrcode.GetLength(1) - 1 - 6, j] = noir;
                qrcode[qrcode.GetLength(1) - 1 - 6, j + 1] = blanc;
            }
            #endregion

            //-----------------------------------carré noir---------------------------------------------------

            qrcode[7, 8] = noir;

            #region motif qr code 25
            //-----------------------------------carré taille 25-----------------------------------------------
            if (length == 25)
            {
                //grand carré noir

                for (int i = 4; i <= 8; i++)
                {
                    for (int j = qrcode.GetLength(0) - 9; j <= qrcode.GetLength(0) - 5; j++)
                    {
                        qrcode[i, j] = noir;
                    }
                }

                //carré blanc

                for (int i = 5; i <= 7; i++)
                {
                    for (int j = qrcode.GetLength(0) - 8; j <= qrcode.GetLength(0) - 6; j++)
                    {
                        qrcode[i, j] = blanc;
                    }
                }

                //petit carré noir

                qrcode[6, qrcode.GetLength(0) - 7] = noir;

            }
            #endregion

            #region cases bleues

            byte[] bleu = { 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0 };

            //ligne bas vertical gauche
            for (int i = 0; i < 7; i++)
            {
                if (bleu[i] == 1)
                {
                    qrcode[i, 8] = noir;
                }
                else
                {
                    qrcode[i, 8] = blanc;
                }

            }

            //ligne haut vertical gauche
            int compte = 7;
            for (int i = qrcode.GetLength(0) - 9; i <= qrcode.GetLength(1) - 1; i++)
            {
                if (qrcode[i, 8].R != 0)
                {
                    if (bleu[compte] == 1)
                    {
                        qrcode[i, 8] = noir;
                    }
                    else
                    {
                        qrcode[i, 8] = blanc;
                    }
                    compte++;
                }
            }
            int compte3 = 0;
            //ligne gauche haute horizontale
            for (int j = 0; j <= 7; j++)
            {
                if (qrcode[qrcode.GetLength(1) - 9, j].R != 0)
                {
                    if (bleu[compte3] == 1)
                    {
                        qrcode[qrcode.GetLength(1) - 9, j] = noir;
                    }
                    else
                    {
                        qrcode[qrcode.GetLength(1) - 9, j] = blanc;
                    }
                    compte3++;
                }
            }


            //ligne haut horizontale droite
            int compte2 = 7;
            for (int j = qrcode.GetLength(1) - 8; j <= qrcode.GetLength(1) - 1; j++)
            {
                if (bleu[compte2] == 1)
                {
                    qrcode[qrcode.GetLength(1) - 9, j] = noir;
                }
                else
                {
                    qrcode[qrcode.GetLength(1) - 9, j] = blanc;
                }
                compte2++;
            }
            #endregion

            #region remplissage avec la chaine de caractère

            string chaineenbinaire = ConversionChaineEnNumero044(chaine); //récupère la chaine encodée
            int comptage = 0;

            for (int j = qrcode.GetLength(1) - 1; j > 0; j = j - 2)
            {
                if (j == 6)
                {
                    j--;      //correspond à la colonne complètement remplis avec les carrés et la ligne pointillés (à sauter)
                }
                if (j % 4 == 0 && j > 5) //à droite de la ligne pointillée et on doit monter
                {
                    //montée
                    for (int i = 0; i < qrcode.GetLength(0); i++)
                    {
                        if (qrcode[i, j].R != 255 && qrcode[i, j].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                        {
                            if ((i + j) % 2 != 0)  //masquage
                            {
                                if (chaineenbinaire[comptage] == '1')
                                {
                                    qrcode[i, j] = noir;
                                }
                                else
                                {
                                    qrcode[i, j] = blanc;
                                }
                            }
                            else
                            {
                                if (chaineenbinaire[comptage] == '1')
                                {
                                    qrcode[i, j] = blanc;
                                }
                                else
                                {
                                    qrcode[i, j] = noir;
                                }
                            }
                            comptage++;
                        }
                        if (qrcode[i, j - 1].R != 255 && qrcode[i, j - 1].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                        {
                            if ((i + j - 1) % 2 != 0) //masquage
                            {
                                if (chaineenbinaire[comptage] == '1')
                                {
                                    qrcode[i, j - 1] = noir;
                                }
                                else
                                {
                                    qrcode[i, j - 1] = blanc;
                                }
                            }
                            else
                            {
                                if (chaineenbinaire[comptage] == '1')
                                {
                                    qrcode[i, j - 1] = blanc;
                                }
                                else
                                {
                                    qrcode[i, j - 1] = noir;
                                }
                            }
                            comptage++;
                        }
                    }

                }
                else
                {
                    if (j > 5 && j % 2 == 0) //on est à droite de la ligne pointillée et sur une descente dans ce cas
                    {

                        //descente
                        for (int i = qrcode.GetLength(0) - 1; i >= 0; i--)
                        {
                            if (qrcode[i, j].R != 255 && qrcode[i, j].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                            {
                                if ((i + j) % 2 != 0) //masquage
                                {

                                    if (chaineenbinaire[comptage] == '1')
                                    {
                                        qrcode[i, j] = noir;
                                    }
                                    else
                                    {
                                        qrcode[i, j] = blanc;
                                    }
                                }
                                else
                                {
                                    if (chaineenbinaire[comptage] == '1')
                                    {
                                        qrcode[i, j] = blanc;
                                    }
                                    else
                                    {
                                        qrcode[i, j] = noir;
                                    }
                                }
                                comptage++;
                            }
                            //rempli la case de gauche
                            if (qrcode[i, j - 1].R != 255 && qrcode[i, j - 1].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                            {
                                if ((i + j - 1) % 2 != 0) //masquage
                                {

                                    if (chaineenbinaire[comptage] == '1')
                                    {
                                        qrcode[i, j - 1] = noir;
                                    }
                                    else
                                    {
                                        qrcode[i, j - 1] = blanc;
                                    }
                                }
                                else
                                {
                                    if (chaineenbinaire[comptage] == '1')
                                    {
                                        qrcode[i, j - 1] = blanc;
                                    }
                                    else
                                    {
                                        qrcode[i, j - 1] = noir;
                                    }
                                }
                                comptage++;
                            }
                        }
                    }
                    else
                    {
                        if (j == 5) //si on est arrivés à gauche de la ligne pointillée il ne reste plus que 2 montées et une descente quel que soit la version de qr code
                        {
                            //descente
                            for (int i = qrcode.GetLength(0) - 1; i >= 0; i--)
                            {
                                if (qrcode[i, j].R != 255 && qrcode[i, j].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                                {
                                    if ((i + j) % 2 != 0) //masquage
                                    {

                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j] = noir;
                                        }
                                        else
                                        {
                                            qrcode[i, j] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j] = blanc;
                                        }
                                        else
                                        {
                                            qrcode[i, j] = noir;
                                        }
                                    }
                                    comptage++;
                                }
                                //rempli la case de gauche
                                if (qrcode[i, j - 1].R != 255 && qrcode[i, j - 1].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                                {
                                    if ((i + j - 1) % 2 != 0) //masquage
                                    {

                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j - 1] = noir;
                                        }
                                        else
                                        {
                                            qrcode[i, j - 1] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j - 1] = blanc;
                                        }
                                        else
                                        {
                                            qrcode[i, j - 1] = noir;
                                        }
                                    }
                                    comptage++;
                                }
                            }
                            j = j - 2;
                            //montée
                            for (int i = 0; i < qrcode.GetLength(0); i++)
                            {
                                if (qrcode[i, j].R != 255 && qrcode[i, j].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                                {
                                    if ((i + j) % 2 != 0) //masquage
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j] = noir;
                                        }
                                        else
                                        {
                                            qrcode[i, j] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j] = blanc;
                                        }
                                        else
                                        {
                                            qrcode[i, j] = noir;
                                        }
                                    }
                                    comptage++;
                                }
                                //rempli la case de gauche
                                if (qrcode[i, j - 1].R != 255 && qrcode[i, j - 1].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                                {
                                    if ((i + j - 1) % 2 != 0) //masquage
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j - 1] = noir;
                                        }
                                        else
                                        {
                                            qrcode[i, j - 1] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j - 1] = blanc;
                                        }
                                        else
                                        {
                                            qrcode[i, j - 1] = noir;
                                        }
                                    }
                                    comptage++;
                                }
                            }
                            j = j - 2;
                            //descente
                            for (int i = qrcode.GetLength(0) - 1; i >= 0; i--)
                            {
                                if (qrcode[i, j].R != 255 && qrcode[i, j].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                                {
                                    if ((i + j) % 2 != 0) //masquage
                                    {

                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j] = noir;
                                        }
                                        else
                                        {
                                            qrcode[i, j] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j] = blanc;
                                        }
                                        else
                                        {
                                            qrcode[i, j] = noir;
                                        }
                                    }
                                    comptage++;
                                }
                                //rempli la case de gauche
                                if (qrcode[i, j - 1].R != 255 && qrcode[i, j - 1].R != 0) //si la case n'est pas déjà rempli par un motif donc pas noir et blanc
                                {
                                    if ((i + j - 1) % 2 != 0) //masquage
                                    {

                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j - 1] = noir;
                                        }
                                        else
                                        {
                                            qrcode[i, j - 1] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (chaineenbinaire[comptage] == '1')
                                        {
                                            qrcode[i, j - 1] = blanc;
                                        }
                                        else
                                        {
                                            qrcode[i, j - 1] = noir;
                                        }
                                    }
                                    comptage++;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            //met à jour les attributs de l'image créée
            this.image = qrcode;
            this.height = length;
            this.width = length;
            this.taillefichier = this.width * this.height * 3;
        }


        /// <summary>
        /// prend la chaine de caractere et la convertit en binaire de 152 ou 272 bits suivant qrcode
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>
        public string ConversionChaineEnNumero044(string chaine)
        {

            int bit = 0;     //paramètre qui détermine la longueur du tableau
            if (chaine != null && chaine.Length != 0)
            {
                if (chaine.Length <= 25)     //choix de la version 1
                {
                    bit = 152;
                }
                else
                {
                    if (chaine.Length <= 47)  //choix de la version 2 
                    {
                        bit = 272;
                    }

                }
            }

            int[] retour = new int[bit];
            string resultat = "";                               //création d'un string car plus facile à concataner puis conversion par la suite en tableau
            int nombre = 0;                                     // conversion des lettre en base 10
            char[] chainecassee = chaine.ToCharArray();         //transforme le string en entrée en tableau de char

            //------------------------------------------------------------Etape ajout encodeur : code alphanumérique et la taille de la chaine en binaire--------------------------------------------------------------------

            resultat += Encodeur(chaine);


            //-----------------------------------------------------------------------Ajout de la chaine de caractere transformée---------------------------------------------------------------


            //boucle pour convertir les lettres par deux en binaire

            int fin = 0;
            if (chainecassee.Length % 2 != 0) fin = 1; //pour faire soit le cas ou le nombre de caractère est pair ou impair


            for (int i = 0; i < chainecassee.Length - fin; i++)
            {
                if (i % 2 != 0) //cas où l'indice de la case est impair donc on est sur le deuxième nombre du binôme
                {
                    nombre += ConversionAlphaNumérique(chainecassee[i]);      //on ajoute le caractère convertit en alphanumérique *45^0 donc *1
                    for (int g = 0; g < ConversionEntierToBinaire(nombre, 11).Length; g++)
                    {
                        resultat += Convert.ToString(ConversionEntierToBinaire(nombre, 11)[g]);  //on ajoute à la chaine le résultat obtenu en binaire
                    }
                    nombre = 0; //on remet à 0 pour les étapes suivantes
                }
                else
                {
                    nombre += 45 * ConversionAlphaNumérique(chainecassee[i]); //cas où l'indice est pair donc c'est le premier du binôme donc
                    //on ajoute donc 45*le caractère convertit en alphanumérique
                }
            }

            //si le nombre total de caractère est impair, le dernier caractère doit être multiplié par 45^0 et non 45^1
            if (chainecassee.Length % 2 != 0)
            {
                for (int i = 0; i < ConversionEntierToBinaire(ConversionAlphaNumérique(chainecassee[chainecassee.Length - 1]), 6).Length; i++)
                {
                    resultat += Convert.ToString(ConversionEntierToBinaire(ConversionAlphaNumérique(chainecassee[chainecassee.Length - 1]), 6)[i]); //on ajoute donc au résultat après avoir convertit
                }
            }
            //-----------------------------Etape terminaison-------------------------------------------------------------------

            //Ajout de la terminaison
            int terminaison = bit - resultat.Length;            //selon la différence avec 152 ou 272 en ajoute de 1 à 4 "0" à droite
            if (terminaison < 4)
            {
                for (int i = 0; i < terminaison; i++)
                {
                    resultat += "0";
                }
            }
            else
            {
                resultat += "0000";                             //si la différence est >= 4 on ajoute systématique "0000" 
            }

            // vérifions si la taille du resultat est modulo 8
            if (resultat.Length % 8 != 0)          //si ce n'est pas le cas on ajoute autant de 0 qu'il faut pour que ce soit multiple de 8
            {
                int reste = 8 - (resultat.Length % 8);
                for (int i = 0; i < reste; i++)
                {
                    resultat += "0";
                }
            }

            //compléter le reste jusqu'à atteindre 152 ou 272
            if (resultat.Length < bit)
            {
                string b236 = "11101100";
                string b17 = "00010001";
                int reste = (bit - resultat.Length) / 8;
                for (int i = 0; i < reste; i++)
                {
                    if (i % 2 == 0)                                    //si nombre paire on ajoute b236 sinon b17
                    {
                        resultat += b236;
                    }
                    else
                    {
                        resultat += b17;
                    }

                }

            }

            //-----------------------------------correcteur reed solomon-----------------------------------------------------

            if (bit == 152)
            {
                resultat += ReedSolomon(resultat, 7); //pour le qr code version 1
            }
            else
            {
                resultat += ReedSolomon(resultat, 10); //pour le qr code version 2
                resultat += "0000000";
            }

            return resultat;
        }


        /// <summary>
        /// renvoie la terminaison du correcteur a ajouter à la chaine encodée
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>
        public string ReedSolomon(string chaine, int nb)
        {
            //récupérer chaine 154 bits puis convertir par 8 bits en entier (séparer le truc correctement) puis utiliser reed solomon puis convertir en binaire
            int[] binaire8bits = new int[8];
            byte tabentiers = 0;
            int k = 0;
            int l = 0;
            byte[] entreereedsolomon = new byte[chaine.Length / 8]; //la taille dépend de la taille de la chaine donc de la version du qr code
            for (int i = 0; i < chaine.Length; i++)
            {
                k = 0;
                for (int a = i; a < i + 8; a++)  //on récupère la chaine et on la sépare en tableau de 8 bits pour les convertir en entiers
                {
                    if (chaine[a] == '1')
                    {
                        binaire8bits[k] = 1;
                    }
                    else
                    {
                        binaire8bits[k] = 0;
                    }
                    k++;
                }
                i = i + 7;
                tabentiers = Convert.ToByte(ConversionBinaireToEntier(binaire8bits));
                entreereedsolomon[l] = tabentiers; //une fois le tableau créé on l'ajoute à la case correspondante du tableau que l'on va utiliser en entree de la fonction reed solomon encode
                k++;
                l++;
            }

            byte[] result = ReedSolomonAlgorithm.Encode(entreereedsolomon, nb, ErrorCorrectionCodeType.QRCode); //on récupère le tableau d'entiers en sortie de reed solomon encode(nb =7 ou 10 suivant version)

            string retour = "";


            for (int i = 0; i < 7; i++) //on récupère les entiers et on les reconvertit en binaire pour les ajouter à la chaine
            {
                retour += ConversionEntierToBinaire(result[i], 8)[0];
                retour += ConversionEntierToBinaire(result[i], 8)[1];
                retour += ConversionEntierToBinaire(result[i], 8)[2];
                retour += ConversionEntierToBinaire(result[i], 8)[3];
                retour += ConversionEntierToBinaire(result[i], 8)[4];
                retour += ConversionEntierToBinaire(result[i], 8)[5];
                retour += ConversionEntierToBinaire(result[i], 8)[6];
                retour += ConversionEntierToBinaire(result[i], 8)[7];
            }
            if (nb == 10) //on récupère les 3 derniers si on est dans la version 2
            {
                for (int i = 7; i < 10; i++)
                {
                    retour += ConversionEntierToBinaire(result[i], 8)[0];
                    retour += ConversionEntierToBinaire(result[i], 8)[1];
                    retour += ConversionEntierToBinaire(result[i], 8)[2];
                    retour += ConversionEntierToBinaire(result[i], 8)[3];
                    retour += ConversionEntierToBinaire(result[i], 8)[4];
                    retour += ConversionEntierToBinaire(result[i], 8)[5];
                    retour += ConversionEntierToBinaire(result[i], 8)[6];
                    retour += ConversionEntierToBinaire(result[i], 8)[7];
                }
            }
            return retour;
        }


        /// <summary>
        /// fonction comportant le switchcase conversion alphanumérique
        /// </summary>
        /// <param name="lettre"></param>
        /// <returns></returns>
        public int ConversionAlphaNumérique(char lettre)
        {
            int retour = 0;
            //traite chacun des cas et renvoie le numéro correspondant au caractère donné en entrée
            switch (lettre)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    retour = Convert.ToInt32(lettre);
                    break;
                case 'A':
                case 'a':
                    retour = 10;
                    break;
                case 'B':
                case 'b':
                    retour = 11;
                    break;
                case 'C':
                case 'c':
                    retour = 12;
                    break;
                case 'D':
                case 'd':
                    retour = 13;
                    break;
                case 'E':
                case 'e':
                    retour = 14;
                    break;
                case 'F':
                case 'f':
                    retour = 15;
                    break;
                case 'G':
                case 'g':
                    retour = 16;
                    break;
                case 'H':
                case 'h':
                    retour = 17;
                    break;
                case 'I':
                case 'i':
                    retour = 18;
                    break;
                case 'J':
                case 'j':
                    retour = 19;
                    break;
                case 'K':
                case 'k':
                    retour = 20;
                    break;
                case 'L':
                case 'l':
                    retour = 21;
                    break;
                case 'M':
                case 'm':
                    retour = 22;
                    break;
                case 'N':
                case 'n':
                    retour = 23;
                    break;
                case 'O':
                case 'o':
                    retour = 24;
                    break;
                case 'P':
                case 'p':
                    retour = 25;
                    break;
                case 'Q':
                case 'q':
                    retour = 26;
                    break;
                case 'R':
                case 'r':
                    retour = 27;
                    break;
                case 'S':
                case 's':
                    retour = 28;
                    break;
                case 'T':
                case 't':
                    retour = 29;
                    break;
                case 'U':
                case 'u':
                    retour = 30;
                    break;
                case 'V':
                case 'v':
                    retour = 31;
                    break;
                case 'W':
                case 'w':
                    retour = 32;
                    break;
                case 'X':
                case 'x':
                    retour = 33;
                    break;
                case 'Y':
                case 'y':
                    retour = 34;
                    break;
                case 'Z':
                case 'z':
                    retour = 35;
                    break;
                case ' ':
                    retour = 36;
                    break;
                case '$':
                    retour = 37;
                    break;
                case '%':
                    retour = 38;
                    break;
                case '*':
                    retour = 39;
                    break;
                case '+':
                    retour = 40;
                    break;
                case '-':
                    retour = 41;
                    break;
                case '.':
                    retour = 42;
                    break;
                case '/':
                    retour = 43;
                    break;
                case ':':
                    retour = 44;
                    break;
            }
            return retour;
        }

        #endregion

        #region lecture qrcode

        /// <summary>
        /// renvoie la chaine de caractère encodé dans le qr code
        /// </summary>
        /// <param name="QR"></param>
        /// <returns></returns>
        public string LectureQR(MyImage QR)
        {
            Pixel[,] qrcode = new Pixel[QR.Height, QR.Width];       //conversion de l'image en matrice de pixel pour traitement

            for (int i = 0; i < qrcode.GetLength(0); i++)
            {
                for (int j = 0; j < qrcode.GetLength(1); j++)
                {
                    qrcode[i, j] = QR.image[i, j];          //on remplit notre matrice de pixel avec le qr code donné en paramètre
                }
            }

            bool sens = true;
            bool sortie = false;                //permettre de répéter la boucle avec la rotation de l'image
                                                //jusqu'à que le QR soit dans le bon sens

            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);

            #region Placer le QR dans le bon sens

            while (sortie == false)
            {
                int compteur = 0;

                for (int i = 2; i <= 4; i++)
                {
                    for (int j = qrcode.GetLength(0) - 5; j <= qrcode.GetLength(0) - 3; j++)
                    {
                        if (qrcode[i, j] == noir)
                        {
                            compteur++;
                        }
                    }
                }
                if (compteur == 9) sens = false;


                if (sens == false)
                {
                    QR.Rotation(90);
                }
                else
                {
                    sortie = true;
                }
            }
            #endregion

            #region Masquer tous les identificateur du QR

            //attention lecture effectué de bas en haut
            Pixel couleur = new Pixel(125, 125, 125);
            //colorer le motif en bas à gauche 
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j <= 8; j++)
                {
                    qrcode[i, j] = couleur;
                }
            }
            //colorer le motif en haut à droite
            for (int i = qrcode.GetLength(1) - 9; i <= qrcode.GetLength(1) - 1; i++)
            {
                for (int j = 0; j <= 8; j++)
                {
                    qrcode[i, j] = couleur;
                }
            }

            //colorer le motif en haut à gauche
            for (int i = qrcode.GetLength(1) - 9; i <= qrcode.GetLength(1) - 1; i++)
            {
                for (int j = qrcode.GetLength(0) - 8; j <= qrcode.GetLength(0) - 1; j++)
                {
                    qrcode[i, j] = couleur;
                }
            }
            //ligne de synchronisation vertical
            for (int i = 8; i <= 11; i++)
            {
                qrcode[i, 6] = couleur;
            }
            //ligne de synchronisation hori
            for (int j = 8; j <= qrcode.GetLength(0) - 9; j++)
            {
                qrcode[qrcode.GetLength(1) - 1 - 6, j] = couleur;
            }
            //carré dans la version 2 du QR
            if (qrcode.GetLength(0) == 25)
            {

                for (int i = 4; i <= 8; i++)
                {
                    for (int j = qrcode.GetLength(0) - 9; j <= qrcode.GetLength(0) - 5; j++)
                    {
                        qrcode[i, j] = couleur;
                    }
                }
            }
            #endregion

            #region décodage

            string decodage = "";

            for (int j = qrcode.GetLength(1) - 1; j > 0; j = j - 2)
            {
                if (j == 6)
                {
                    j--;      //correspond à la colonne complètement remplis avec les carrés et la ligne pointillés (à sauter)
                }
                if (j % 4 == 0 && j > 5) //à droite de la ligne pointillée et on doit monter
                {
                    //montée
                    for (int i = 0; i < qrcode.GetLength(0); i++)
                    {
                        if (qrcode[i, j].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                        {
                            if ((i + j) % 2 != 0)  //masquage
                            {
                                if (qrcode[i, j].R == 0)
                                {
                                    decodage += '1';
                                }
                                else
                                {
                                    decodage += '0';
                                }
                            }
                            else
                            {
                                if (qrcode[i, j].R == 255)
                                {
                                    decodage += '1';
                                }
                                else
                                {
                                    decodage += '0';
                                }
                            }
                        }
                        //case à gauche
                        if (qrcode[i, j - 1].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                        {
                            if ((i + j - 1) % 2 != 0)  //masquage
                            {
                                if (qrcode[i, j - 1].R == 0)
                                {
                                    decodage += '1';
                                }
                                else
                                {
                                    decodage += '0';
                                }
                            }
                            else
                            {
                                if (qrcode[i, j - 1].R == 255)
                                {
                                    decodage += '1';
                                }
                                else
                                {
                                    decodage += '0';
                                }
                            }
                        }
                    }

                }
                else
                {
                    if (j > 5 && j % 2 == 0) //on est à droite de la ligne pointillée et sur une descente dans ce cas
                    {

                        //descente
                        for (int i = qrcode.GetLength(0) - 1; i >= 0; i--)
                        {
                            if (qrcode[i, j].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                            {
                                if ((i + j) % 2 != 0)  //masquage
                                {
                                    if (qrcode[i, j].R == 0)
                                    {
                                        decodage += '1';
                                    }
                                    else
                                    {
                                        decodage += '0';
                                    }
                                }
                                else
                                {
                                    if (qrcode[i, j].R == 255)
                                    {
                                        decodage += '1';
                                    }
                                    else
                                    {
                                        decodage += '0';
                                    }
                                }
                            }
                            //rempli la case de gauche
                            if (qrcode[i, j - 1].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                            {
                                if ((i + j - 1) % 2 != 0)  //masquage
                                {
                                    if (qrcode[i, j - 1].R == 0)
                                    {
                                        decodage += '1';
                                    }
                                    else
                                    {
                                        decodage += '0';
                                    }
                                }
                                else
                                {
                                    if (qrcode[i, j - 1].R == 255)
                                    {
                                        decodage += '1';
                                    }
                                    else
                                    {
                                        decodage += '0';
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (j == 5) //si on est arrivés à gauche de la ligne pointillée il ne reste plus que 2 montées et une descente quel que soit la version de qr code
                        {
                            //descente
                            for (int i = qrcode.GetLength(0) - 1; i >= 0; i--)
                            {
                                if (qrcode[i, j].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                                {
                                    if ((i + j) % 2 != 0)  //masquage
                                    {
                                        if (qrcode[i, j].R == 0)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                    else
                                    {
                                        if (qrcode[i, j].R == 255)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                }
                                //rempli la case de gauche
                                if (qrcode[i, j - 1].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                                {
                                    if ((i + j - 1) % 2 != 0)  //masquage
                                    {
                                        if (qrcode[i, j - 1].R == 0)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                    else
                                    {
                                        if (qrcode[i, j - 1].R == 255)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                }
                            }
                            j = j - 2;
                            //montée
                            for (int i = 0; i < qrcode.GetLength(0); i++)
                            {
                                if (qrcode[i, j].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                                {
                                    if ((i + j) % 2 != 0)  //masquage
                                    {
                                        if (qrcode[i, j].R == 0)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                    else
                                    {
                                        if (qrcode[i, j].R == 255)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                }
                                //rempli la case de gauche
                                if (qrcode[i, j - 1].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                                {
                                    if ((i + j - 1) % 2 != 0)  //masquage
                                    {
                                        if (qrcode[i, j - 1].R == 0)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                    else
                                    {
                                        if (qrcode[i, j - 1].R == 255)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                }
                            }
                            j = j - 2;
                            //descente
                            for (int i = qrcode.GetLength(0) - 1; i >= 0; i--)
                            {
                                if (qrcode[i, j].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                                {
                                    if ((i + j) % 2 != 0)  //masquage
                                    {
                                        if (qrcode[i, j].R == 0)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                    else
                                    {
                                        if (qrcode[i, j].R == 255)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                }
                                //rempli la case de gauche
                                if (qrcode[i, j - 1].R != 125) //si la case n'est pas déjà rempli par un motif donc n'est pas couleur
                                {
                                    if ((i + j - 1) % 2 != 0)  //masquage
                                    {
                                        if (qrcode[i, j - 1].R == 0)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                    else
                                    {
                                        if (qrcode[i, j - 1].R == 255)
                                        {
                                            decodage += '1';
                                        }
                                        else
                                        {
                                            decodage += '0';
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

                #region conversion chaine
                //string result = "";
                //reedsolomon verifie chaine bonne
            if(qrcode.GetLength(0)==21) decodage = DecodeReedSolomon(decodage,7);
            else decodage = DecodeReedSolomon(decodage, 7);
            //les quatre premiers caractères sont 0010 pour code alphanumérique, donc inutile dans notre cas
            string result = ConversionNumero044EnChaine(decodage);


            #endregion

            return result;
        }


        /// <summary>
        /// fonction comportant le switchcase conversion char depuis alphanumérique
        /// </summary>
        /// <param name="lettre"></param>
        /// <returns></returns>
        public char ConversionAlphaNumériqueToChar(int nombre)
        {
            char retour = '0';
            //traite chacun des cas et renvoie le numéro correspondant au caractère donné en entrée
            switch (nombre)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    retour = Convert.ToChar(nombre);
                    break;
                case 10:
                    retour = 'a';
                    break;
                case 11:
                    retour = 'b';
                    break;
                case 12:
                    retour = 'c';
                    break;
                case 13:
                    retour = 'd';
                    break;
                case 14:
                    retour = 'e';
                    break;
                case 15:
                    retour = 'f';
                    break;
                case 16:
                    retour = 'g';
                    break;
                case 17:
                    retour = 'h';
                    break;
                case 18:
                    retour = 'i';
                    break;
                case 19:
                    retour = 'j';
                    break;
                case 20:
                    retour = 'k';
                    break;
                case 21:
                    retour = 'l';
                    break;
                case 22:
                    retour = 'm';
                    break;
                case 23:
                    retour = 'n';
                    break;
                case 24:
                    retour = 'o';
                    break;
                case 25:
                    retour = 'p';
                    break;
                case 26:
                    retour = 'q';
                    break;
                case 27:
                    retour = 'r';
                    break;
                case 28:
                    retour = 's';
                    break;
                case 29:
                    retour = 't';
                    break;
                case 30:
                    retour = 'u';
                    break;
                case 31:
                    retour = 'v';
                    break;
                case 32:
                    retour = 'w';
                    break;
                case 33:
                    retour = 'x';
                    break;
                case 34:
                    retour = 'y';
                    break;
                case 35:
                    retour = 'z';
                    break;
                case 36:
                    retour = ' ';
                    break;
                case 37:
                    retour = '$';
                    break;
                case 38:
                    retour = '%';
                    break;
                case 39:
                    retour = '*';
                    break;
                case 40:
                    retour = '+';
                    break;
                case 41:
                    retour = '-';
                    break;
                case 42:
                    retour = '.';
                    break;
                case 43:
                    retour = '/';
                    break;
                case 44:
                    retour = ':';
                    break;
            }
            return retour;
        }

        
        /// <summary>
        /// prend la chaine de caractere encodee et la convertit en chaine decodee
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>
        public string ConversionNumero044EnChaine(string decodage)
        {

            int bit = 0;     //paramètre qui détermine la longueur du tableau
            int[] taille = new int[9];
            for (int i = 4; i < 13; i++)
            {
                if (decodage[i] == '1')
                {
                    taille[i - 4] = 1;
                }
                else
                {
                    taille[i - 4] = 0;
                }
            }
            int tailleint = ConversionBinaireToEntier(taille);
            if (tailleint <= 25)     //choix de la version 1
            {
                bit = 152;
            }
            else
            {
                if (tailleint <= 47)  //choix de la version 2 
                {
                    bit = 272;
                }

            }
            string chaineencodee = "";
            //on récupère uniquement la partie chaine de caractère encodée du string :
            for (int i = 13; i < bit; i++)
            {
                chaineencodee += decodage[i];
            }

            //on retire les 236 et 17 encodés en binaire à la fin de la chaine
            bool term = true;
            string b23617 = "";
            string sansterm = "";
            int nbfoisaenlever = 1;
            while (term == true)
            {
                b23617 = "";
                for (int i = chaineencodee.Length - 8 * nbfoisaenlever; i < chaineencodee.Length - 8 * (nbfoisaenlever - 1); i++)
                {
                    b23617 += chaineencodee[i];
                }
                if (b23617 == "11101100" || b23617 == "00010001")
                {
                    nbfoisaenlever++;

                }
                else
                {
                    for (int u = 0; u < chaineencodee.Length - 8 * (nbfoisaenlever - 1); u++)
                    {
                        sansterm += chaineencodee[u];
                    }
                    term = false;
                }
            }
            //dépend si la chaine initiale à un nombre pair ou impair de caractère : codés sur 11 bits sauf si impair où c'est sur 6 bits
            //on veut maintenant enlever les 00 ajoutés pour rendre la chaine multiple de 4
            bool impair = false;
            string sanszeros = "";
            int nbbinome = 0;
            if (tailleint % 2 != 0) impair = true;
            if (impair == false)
            {
                nbbinome = (int)(tailleint / 2);
                for (int i = 0; i < (int)(tailleint / 2) * 11; i++)
                {
                    sanszeros += sansterm[i];
                }
            }
            else
            {
                nbbinome = (int)(tailleint / 2) + 1;
                for (int o = 0; o < (int)(tailleint / 2) * 11 + 6; o++)
                {
                    sanszeros += sansterm[o];
                }
            }

            //maintenant qu'on a la chaine avec uniquement en binaire la chaine de caractère encodées,
            //on sépare en tableau de 11 ou 6 pour impair puis on convertit cela en entier
            //et on les stocke dans un tableau
            int[] tabentiers = new int[nbbinome];
            int compteur = 0;
            for (int i = 0; i < sanszeros.Length; i++)
            {
                int compt = 0;
                if (i + 10 >= sanszeros.Length)
                {
                    int[] tab6 = new int[6];
                    for (int j = i; j < i + 6; j++)
                    {
                        if (sanszeros[j] == '1')
                        {
                            tab6[compt] = 1;
                        }
                        else
                        {
                            tab6[compt] = 0;
                        }
                        compt++;
                    }
                    tabentiers[compteur] = ConversionBinaireToEntier(tab6);
                }
                else
                {
                    int[] tab11 = new int[11];
                    for (int j = i; j < i + 11; j++)
                    {
                        if (sanszeros[j] == '1')
                        {
                            tab11[compt] = 1;
                        }
                        else
                        {
                            tab11[compt] = 0;
                        }
                        compt++;
                    }
                    tabentiers[compteur] = ConversionBinaireToEntier(tab11);
                }
                compteur++;
                i = i + 10;

            }
            //on doit maintenant à partir des entiers retrouver les deux lettres auquel cela correspond chaque fois
            string resultat = "";
            int[] tabinter = new int[2];
            for(int i = 0; i < tabentiers.Length - 1; i++)
            {
                tabinter = Conversion45toAlphanumerique(tabentiers[i]);
                resultat += ConversionAlphaNumériqueToChar(tabinter[0]);
                resultat += ConversionAlphaNumériqueToChar(tabinter[1]);
            }
            //on s'occupe du dernier :
            if (tailleint % 2 != 0)
            {
                resultat += ConversionAlphaNumériqueToChar(tabentiers[tabentiers.Length - 1]);
            }
            else
            {
                tabinter = Conversion45toAlphanumerique(tabentiers[tabentiers.Length - 1]);
                resultat += ConversionAlphaNumériqueToChar(tabinter[0]);
                resultat += ConversionAlphaNumériqueToChar(tabinter[1]);
            }
            return resultat;
        }

        /// <summary>
        /// renvoie la terminaison du correcteur a ajouter à la chaine encodée
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>
        public string DecodeReedSolomon(string chainecomplete, int nb)
        {
            //nb =7 ou 10 suivant version
            //récupérer chaine 154 bits puis convertir par 8 bits en entier (séparer le truc correctement) puis utiliser reed solomon puis convertir en binaire
            int[] binaire8bits = new int[8];
            int taille = 0;
            if (nb == 7) taille = 152;
            if (nb == 10) taille = 272;
            //on récupère la chaine de caractère
            string chaine = "";
            for(int i = 0; i < taille;i++)
            {
                chaine += chainecomplete[i];
            }

            //on récupère le code de correction reed solomon
            string code = "";
            for (int i = taille; i < chainecomplete.Length; i++)
            {
                code += chainecomplete[i];
            }

            //pour avoir la chaine en entiers
            byte tabentiers = 0;
            int k = 0;
            int l = 0;
            byte[] entreereedsolomon = new byte[chaine.Length / 8]; //la taille dépend de la taille de la chaine donc de la version du qr code
            
            for (int i = 0; i < chaine.Length; i++)
            {
                k = 0;
                for (int a = i; a < i + 8; a++)  //on récupère la chaine et on la sépare en tableau de 8 bits pour les convertir en entiers
                {
                    if (chaine[a] == '1')
                    {
                        binaire8bits[k] = 1;
                    }
                    else
                    {
                        binaire8bits[k] = 0;
                    }
                    k++;
                }
                i = i + 7;
                tabentiers = Convert.ToByte(ConversionBinaireToEntier(binaire8bits));
                entreereedsolomon[l] = tabentiers; //une fois le tableau créé on l'ajoute à la case correspondante du tableau que l'on va utiliser en entree de la fonction reed solomon encode
                k++;
                l++;
            }

            //pour avoir le code de correction en entiers
            byte tabentiers2 = 0;
            int k2 = 0;
            int l2 = 0;
            byte[] entreecodereed = new byte[code.Length / 8]; //la taille dépend de la taille de la chaine donc de la version du qr code

            for (int i = 0; i < code.Length; i++)
            {
                k2 = 0;
                for (int a = i; a < i + 8; a++)  //on récupère la chaine et on la sépare en tableau de 8 bits pour les convertir en entiers
                {
                    if (code[a] == '1')
                    {
                        binaire8bits[k2] = 1;
                    }
                    else
                    {
                        binaire8bits[k2] = 0;
                    }
                    k2++;
                }
                i = i + 7;
                tabentiers2 = Convert.ToByte(ConversionBinaireToEntier(binaire8bits));
                entreecodereed[l2] = tabentiers2; //une fois le tableau créé on l'ajoute à la case correspondante du tableau que l'on va utiliser en entree de la fonction reed solomon encode
                k2++;
                l2++;
            }
           
            byte[] result = ReedSolomonAlgorithm.Decode(entreereedsolomon, entreecodereed); //(entreereedsolomon,entreecodereed, ErrorCorrectionCodeType.QRCode); //on récupère le tableau d'entiers en sortie de reed solomon encode(nb =7 ou 10 suivant version)

            string retour = "";


            for (int i = 0; i < result.Length; i++) //on récupère les entiers et on les reconvertit en binaire pour les ajouter à la chaine
            {
                retour += ConversionEntierToBinaire(result[i], 8)[0];
                retour += ConversionEntierToBinaire(result[i], 8)[1];
                retour += ConversionEntierToBinaire(result[i], 8)[2];
                retour += ConversionEntierToBinaire(result[i], 8)[3];
                retour += ConversionEntierToBinaire(result[i], 8)[4];
                retour += ConversionEntierToBinaire(result[i], 8)[5];
                retour += ConversionEntierToBinaire(result[i], 8)[6];
                retour += ConversionEntierToBinaire(result[i], 8)[7];
            }
            return retour;
        }


        /// <summary>
        /// prend un entier en paramètre et le retrouve les coefficients puissance de 45 pour les binomes de lettre qrcode
        /// </summary>
        /// <param name="entier"></param>
        /// <returns></returns>
        public int[] Conversion45toAlphanumerique(int entier)
        {
            int[] retour = new int[2];
            int div1 = entier / 45;
            retour[0] = div1;
            entier = entier - div1 * 45;
            retour[1] = entier;
            return retour;
        }

        #endregion

    }
}
