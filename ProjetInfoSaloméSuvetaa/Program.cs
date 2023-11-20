using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace ProjetInfoSaloméSuvetaa
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Quelle action souhaitez-vous réaliser ? \n1-Manipulation d'image \n2-QR Code " +
                 "\n3-Création d'une fractale \nVeuillez saisir le chiffre correspondant à l'action souhaitée.");
            string action1 = Console.ReadLine();
            string entree = "";
            while (action1 != "1" & action1 != "2" & action1 != "3")
            {
                Console.WriteLine("Saisie incorrecte. Veuillez réessayer.");
                action1 = Console.ReadLine();
            }

            if (action1 == "1")
            {
                //---------------------------------------Vérification de l'existence du fichier--------------------------------------
                bool test = false;
                string nomFichier = "";
                while (test == false)
                {
                    Console.WriteLine("Veuillez saisir le nom de votre fichier sous le nom du fichier d'origine");
                    Console.Write("Voici le nom des quelques fichiers déjà existants: \nlac \ncoco \nlena \nsortie \n");
                    entree = Console.ReadLine();
                    nomFichier = "./" + entree + ".bmp";
                    test = File.Exists(nomFichier);
                    if (test == true)
                    {
                        Console.WriteLine("La saisie est validée.");
                    }
                    else
                    {
                        Console.WriteLine("Saisie incorrecte ou fichier introuvable. \n " +
                            "Veuillez saisir le nom de votre fichier à nouveau.");
                    }

                }
                Console.WriteLine(nomFichier);
                Console.WriteLine();

                //---------------------Afficher les propriétés du fichier--------------------------
                MyImage image = new MyImage(nomFichier);
                //Afficher les propriétés de l'image (header à afficher ?)
                Console.Write("Taille de votre fichier: " + image.Taillefichier + "\nDimension de votre fichier: " + image.Width + " x " + image.Height + "\n");
                Console.WriteLine();

                //---------------------Réalisation des actions-------------------------------------
                bool action = false;
                string saisie;
                //Variable pour utiliser le TryParse
                double n;
                int m;

                while (action == false)
                {
                    Console.Write("Vous pouvez effectuer les actions suivantes: \n A- Filtre noir et blanc \n B- Filtre nuance de gris  " +
                   "\n C- Filtre effet miroir  \n D- Agrandir ou rétrécir l'image \n E- Rotation de l'image \n " +
                   "F- Flouter l'image \n G- Détection du contour  \n H- Filtre repoussage \n I- Renforcement des bords" +
                   "\n J- Afficher l'histogramme  \n K- Cacher une image dans une autre \n Z- Pour retourner au menu précédent");
                    Console.WriteLine();
                    Console.WriteLine("Veuillez saisir la lettre de l'action à réaliser. Pensez à la majuscule.");
                    saisie = Console.ReadLine();
                    while (saisie != "A" & saisie != "B" & saisie != "C" & saisie != "D"
                    & saisie != "E" & saisie != "F" & saisie != "G" & saisie != "H"
                    & saisie != "I" & saisie != "J" & saisie != "K" & saisie != "L" & saisie != "Z")
                    {
                        Console.WriteLine("Saisie incorrect. Veuillez réessayer");
                        saisie = Console.ReadLine();

                    }

                    Console.WriteLine("Saisie réussi");
                    while (saisie == "Z")
                    {
                        test = false;
                        nomFichier = "";
                        while (test == false)
                        {
                            Console.WriteLine("Veuillez saisir le nom de votre fichier sous le nom du fichier d'origine");
                            Console.Write("Voici le nom des quelques fichiers déjà existants: \nlac \ncoco \nlena \nsortie \n");
                            entree = Console.ReadLine();
                            nomFichier = "./" + entree + ".bmp";
                            test = File.Exists(nomFichier);
                            if (test == true)
                            {
                                Console.WriteLine("La saisie est validée");
                            }
                            else
                            {
                                Console.WriteLine("Saisie incorrecte ou fichier introuvable \n " +
                                    "Veuillez saisir le nom de votre fichier à nouveau");
                            }

                        }
                        Console.WriteLine(nomFichier);
                        Console.WriteLine();

                        image = new MyImage(nomFichier);
                        //Afficher les propriétés de l'image (header à afficher ?)
                        Console.Write("Taille de votre fichier: " + image.Taillefichier + "\nDimension de votre fichier: " + image.Width + " x " + image.Height + "\n");
                        Console.WriteLine();

                        Console.Write("Vous pouvez effectuer les action suivante: \n A- Filtre noir et blanc \n B- Filtre nuance de gris  " +
                        "\n C- Filtre effet miroir  \n D- Agrandir ou rétrécir l'image \n E- Rotation de l'image \n " +
                         "F- Flouter l'image \n G- Détection du contour  \n H- Filtre repoussage \n I- Renforcement des bords" +
                        "\n J- Afficher l'histogramme \n K- Cacher une image dans une autre \n L-Filtre négatif \n Z- Pour retourner en arrière");
                        Console.WriteLine();
                        Console.WriteLine("Veuillez saisir la lettre de l'action à réaliser. Pensez à la majuscule.");
                        saisie = Console.ReadLine();
                        while (saisie != "A" & saisie != "B" & saisie != "C" & saisie != "D"
                        & saisie != "E" & saisie != "F" & saisie != "G" & saisie != "H"
                        & saisie != "I" & saisie != "J" & saisie != "K" & saisie != "Z")
                        {
                            Console.WriteLine("Saisie incorrect. Veuillez réessayer");
                            saisie = Console.ReadLine();

                        }

                        Console.WriteLine("Saisie réussi");

                    }

                    switch (saisie)
                    {
                        case "A":
                            image.Noir_et_blanc();
                            image.From_Image_To_File("ImageNoirEtBlanc");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageNoirEtBlanc'");
                            break;
                        case "B":
                            image.Nuances_De_Gris();
                            image.From_Image_To_File("ImageNuanceDeGris");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageNuanceDeGris'");
                            break;
                        case "C":       //effet miroir
                            image.Effet_Miroir();
                            image.From_Image_To_File("ImageEffetMiroir");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageEffetMiroir'");
                            break;
                        case "D":       //agrandir l'image avec coef en entrée
                            Console.WriteLine("Choisissez votre coefficient " +
                                "\nPour rappel, pour rétrécir votre image saisissez un RÉEL entre 0,2 et 1 inclus" +
                                "\nPour agrandir votre image saisissez un RÉEL compris entre 1 et 20 inclus ");

                            string coef = Console.ReadLine();
                            bool nombre = Double.TryParse(coef, out n);                      //on utilise TryPArse pour éviter de faire cracher le programme
                                                                                             //si l'utilisateur ne saisie pas un nombre

                            while (nombre == false)
                            {
                                Console.WriteLine("Vous n'avez pas saisie un nombre. Veuillez réessayer.");         //Vérification pour la première saisie
                                coef = Console.ReadLine();
                                nombre = Double.TryParse(coef, out n);
                            }

                            double coefficient = Convert.ToDouble(coef);

                            while (coefficient < (0.2) || coefficient > 20)
                            {
                                Console.WriteLine("Le coefficient saisie est incorrecte. Veuillez recommencer en faisant attention à respecter les conditions précisées");
                                coef = Console.ReadLine();
                                nombre = Double.TryParse(coef, out n);
                                while (nombre == false)
                                {
                                    Console.WriteLine("Vous n'avez pas saisie un nombre. Veuillez réessayer.");
                                    coef = Console.ReadLine();
                                    nombre = Double.TryParse(coef, out n);
                                }
                                coefficient = Convert.ToDouble(coef);
                            }

                            image.Agrandir_Retrecir(coefficient);
                            image.From_Image_To_File("ImageAgranditOuR");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageAgranditOuR'");
                            break;
                        case "E":       //Rotation de l'image avec coef en entréé
                            Console.WriteLine("Veuillez saisir l'angle de rotation souhaité. Veillez à saisir un ENTIER compris entre 0 et 360");
                            string angle = Console.ReadLine();
                            bool entier = Int32.TryParse(angle, out m);

                            while (entier == false)
                            {
                                Console.WriteLine("Vous n'avez pas saisie un nombre entier. Veuillez réessayer.");
                                angle = Console.ReadLine();
                                entier = Int32.TryParse(angle, out m);
                            }

                            int angleFinal = Convert.ToInt32(angle);

                            while (angleFinal < 0 || angleFinal > 360)
                            {
                                Console.WriteLine("Vous n'avez pas saisie un angle compris dans l'intervalle donné. Veuillez réessayer.");
                                angle = Console.ReadLine();
                                entier = Int32.TryParse(angle, out m);
                                while (entier == false)
                                {
                                    Console.WriteLine("Vous n'avez pas saisie un nombre entier. Veuillez réessayer.");
                                    coef = Console.ReadLine();
                                    nombre = Int32.TryParse(coef, out m);
                                }
                                angleFinal = Convert.ToInt32(angle);
                            }

                            image.Rotation(angleFinal);
                            image.From_Image_To_File("ImageRotation");
                            Console.WriteLine("Modification réalisée avec succés sous le nom 'ImageRotation'");
                            break;
                        case "F":
                            image.Flou();
                            image.From_Image_To_File("ImageFlou");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageFlou'");
                            break;
                        case "G":
                            image.DetectionContour();
                            image.From_Image_To_File("ImageDetectionContour");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageDetectionContour'");
                            break;
                        case "H":
                            image.Repoussage();
                            image.From_Image_To_File("ImageRepoussage");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageRepoussage'");
                            break;
                        case "I":
                            image.RenforcementBord();
                            image.From_Image_To_File("ImageRenforcementBord");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageRenforcementBord'");
                            break;
                        case "J":           //Afficher l'histogramme en distinguant les 4 cas
                            bool test1 = false;

                            //permettre de relancer la boucle si l'utilisateur veut afficher un autre histogramme
                            while (test1 == false)
                            {
                                Console.WriteLine("Vous pouver afficher les histogrammes suivant : \n1-Rouge \n2-Vert \n3-Bleu \n4-Nuance de gris");
                                Console.WriteLine("Que souhaitez-vous afficher ? Choisissez un nombre entre 1 et 4 selon la légende.");
                                string choix = Console.ReadLine();

                                //vérifier si la saisie correspond au choix proposé.
                                while (choix != "1" & choix != "2" & choix != "3" & choix != "4")
                                {
                                    Console.WriteLine("Saisie incorrecte. Veuillez réessayer.");
                                    choix = Console.ReadLine();

                                }

                                switch (choix)
                                {
                                    case "1":
                                        Console.WriteLine("Vous avez choisi d'afficher l'histogramme rouge");
                                        MyImage histoRouge = new MyImage(100, 256, image.HistrogrammeRouge());
                                        histoRouge.From_Image_To_File("HistoRouge");
                                        Console.WriteLine("L'histogramme a été créé avec succés sous le nom 'HistoRouge'");
                                        break;
                                    case "2":
                                        Console.WriteLine("Vous avez choisi d'afficher l'histogramme vert");
                                        MyImage histoVert = new MyImage(100, 256, image.HistrogrammeVert());
                                        histoVert.From_Image_To_File("HistoVert");
                                        Console.WriteLine("L'histogramme a été créé avec succés sous le nom 'HistoVert'");
                                        break;
                                    case "3":
                                        Console.WriteLine("Vous avez choisi d'afficher l'histogramme bleu sous le nom 'HistoBleu'");
                                        MyImage histoBleu = new MyImage(100, 256, image.HistrogrammeBleu());
                                        histoBleu.From_Image_To_File("HistoBleu");
                                        Console.WriteLine("L'histogramme a été créé avec succés");
                                        break;
                                    case "4":
                                        Console.WriteLine("Vous avez choisi d'afficher l'histogramme nuance de gris");
                                        MyImage histoNuanceGris = new MyImage(100, 256, image.HistrogrammeNuanceGris());
                                        histoNuanceGris.From_Image_To_File("HistoNuanceGris");
                                        Console.WriteLine("L'histogramme a été créé avec succés sous le nom 'HistoNuanceGris'");
                                        break;
                                }

                                //Demander si l'utilisateur veux afficher un autre histogramme
                                Console.WriteLine("Saisir 1 si vous souhaitez afficher un autre histogramme. Pour quitter saisissez 0");
                                string fin = Console.ReadLine();
                                while (fin != "0" && fin != "1")
                                {
                                    Console.WriteLine("Saisie inccorect. Veuillez entrer 0 ou 1");
                                    fin = Console.ReadLine();
                                }

                                if (fin == "0")
                                {
                                    test1 = true;
                                }
                            }

                            break;
                        case "K":       //Cacher une image dans une autre

                            //-----------------------Demander à l'utilisateur la deuxième image qu'il souhaite caché------------------------
                            bool restart = false;
                            while (restart == false)
                            {
                                Console.WriteLine("Quelle image souhaitez-vous cacher dans " + nomFichier);
                                bool test2 = false;
                                string nomFichier2 = "";
                                while (test2 == false)
                                {
                                    Console.WriteLine("Veuillez saisir le nom de cet image sous son nom d'origine");
                                    Console.Write("Voici le nom des quelques fichiers déjà existants: \nlac \ncoco \nlena \nsortie \n");
                                    entree = Console.ReadLine();
                                    nomFichier2 = "./" + entree + ".bmp";
                                    test2 = File.Exists(nomFichier2);
                                    if (test2 == true)
                                    {
                                        Console.WriteLine("La saisie est validée");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Saisie incorrecte ou fichier introuvable. \n " +
                                            "Veuillez saisir le nom de votre fichier à nouveau");
                                    }
                                }

                                //--------------------------Vérification des dimensions de l'image---------------------------------------

                                MyImage imageACache = new MyImage(nomFichier2); //image à cacher

                                MyImage image2 = new MyImage("./test.bmp");     //image pour stocker le résultat
                                while (image.Width < imageACache.Width || image.Height < imageACache.Height)
                                {
                                    imageACache.Agrandir_Retrecir(0.5);
                                }
                                MyImage cache = new MyImage(image.Image.GetLength(0), image.Image.GetLength(1), image2.CacherImageDansImage(image.Image, image2.FondBlanc(imageACache.Image, image.Image.GetLength(0), image.Image.GetLength(1))));
                                cache.From_Image_To_File("ImageCache");
                                Console.WriteLine("L'image a été caché avec succès sous le nom ImageCache.");


                                //--------------------------Décacher les images------------------------------------
                                Console.WriteLine("Souhaitez-vous décacher vos images ? Si oui tappez 1 sinon tappez 0.");
                                string decacher = Console.ReadLine();

                                while (decacher != "1" & decacher != "0")
                                {
                                    Console.WriteLine("Saisie incorrecte. Veuillez recommencer.");
                                    decacher = Console.ReadLine();
                                }

                                if (decacher == "1")
                                {
                                    MyImage decacheforte = new MyImage(cache.Image.GetLength(0), cache.Image.GetLength(1), image.DecacherImageForte(cache.Image));
                                    decacheforte.From_Image_To_File("sortieForte");
                                    Console.WriteLine("L'image initiale vient d'être décachée sous le nom 'sortieForte'");
                                    MyImage decachefaible = new MyImage(cache.Image.GetLength(0), cache.Image.GetLength(1), image.DecacherImageFaible(cache.Image));
                                    decachefaible.From_Image_To_File("sortieFaible");
                                    Console.WriteLine("L'image que vous avez caché vient d'être décachée sous le nom 'sortieFaible'");

                                }

                                Console.WriteLine("Souhaitez-vous recommencer avec une nouvelle image ? Si oui tappez 1 sinon tappez 0.");
                                string recommencer = Console.ReadLine();

                                while (recommencer != "1" & recommencer != "0")
                                {
                                    Console.WriteLine("Saisie incorrecte. Veuillez recommencer.");
                                    recommencer = Console.ReadLine();
                                }

                                if (recommencer == "0") restart = true;

                            }
                            break;
                        case "L":
                            image.Negatif();
                            image.From_Image_To_File("ImageNegatif");
                            Console.WriteLine("Votre image a été modifée avec succés sous le nom 'ImageNegatif'");
                            break;
                    }


                    //-------------------------Demander à l'utiisateur si il souhaite modifier une nouvelle image-------------------
                    Console.WriteLine("Si vous souhaitez réaliser une nouvelle action, entrer 1. Pour quittez tapper 0.");
                    int sortie = Convert.ToInt32(Console.ReadLine());
                    while (sortie != 0 && sortie != 1)
                    {
                        Console.WriteLine("Saisie incorrecte. Réessayer");
                        sortie = Convert.ToInt32(Console.ReadLine());

                    }

                    if (sortie == 0)
                    {
                        action = true;

                    }
                    else
                    {
                        bool test2 = false;
                        while (test2 == false)
                        {
                            Console.WriteLine("Veuillez saisir le nom de votre fichier sous le nom du fichier d'origine");
                            Console.Write("Voici le nom des quelques fichiers déjà existants: \nlac \ncoco \nlena \nsortie \n");
                            entree = Console.ReadLine();
                            nomFichier = "./" + entree + ".bmp";
                            test2 = File.Exists(nomFichier);
                            if (test2 == true)
                            {
                                Console.WriteLine("La saisie est validée");
                            }
                            else
                            {
                                Console.WriteLine("Saisie incorrecte ou fichier introuvable \n " +
                                    "Veuillez saisir le nom de votre fichier à nouveau");
                            }

                        }

                        image = new MyImage(nomFichier);
                        Console.Write("Taille de votre fichier: " + image.Taillefichier + "\nDimension de votre fichier: " + image.Width + " x " + image.Height + "\n");
                        Console.WriteLine();

                    }
                }
            }
            if (action1 == "2")          //QR Code
            {
                bool restart = false;

                while (restart == false)
                {
                    Console.WriteLine("Quelle actions souhaitez-vous réaliser ? Veuillez saisir correctement le nombre correspondant. " +
                        "\n1.Génération de QR Code \n2.Lecture de QR");
                    string saisie = Console.ReadLine();
                    while (saisie != "1" & saisie != "2")
                    {
                        Console.WriteLine("Saisie incorrect. Veuillez réessayer");
                        saisie = Console.ReadLine();
                    }

                    if (saisie == "1")   //Cas génération de QR
                    {
                        Console.WriteLine("Saisir le mot que vous souhaitez générer grâce au QR Code. \nAttention tous les caractères spéciaux ne sont pas autorisés. " +
                            "Voici ceux qui peuvent être éventuellement saisie : $,%,*,+,-,.,/,: " +
                            "\nVous ne pouvez entrer que 47 caractères au maximum en tenant compte des espaces.");

                        string chaine = Console.ReadLine();
                        bool test = false;

                        //test si les conditions de longueur sont respectés
                        while (chaine == null || chaine.Length == 0 || chaine.Length > 47)
                        {
                            Console.WriteLine("Votre chaine est vide ou dépasse les 47 caractères autorisés. Veuillez réessayer.");
                            chaine = Console.ReadLine();
                        }

                        //---------------------------------------teste si précence de caractère non autorisé dans la chaine---------------------------------------
                        while (test == false)
                        {
                            bool special = false;
                            for (int i = 0; i < chaine.Length; i++)
                            {
                                if (chaine[i] != 'A' && chaine[i] != 'a' && chaine[i] != 'B' && chaine[i] != 'b' && chaine[i] != 'C' && chaine[i] != 'c' &&
                                    chaine[i] != 'D' && chaine[i] != 'd' && chaine[i] != 'E' && chaine[i] != 'e' && chaine[i] != 'F' && chaine[i] != 'f' &&
                                    chaine[i] != 'G' && chaine[i] != 'g' && chaine[i] != 'H' && chaine[i] != 'h' && chaine[i] != 'I' && chaine[i] != 'i' &&
                                    chaine[i] != 'J' && chaine[i] != 'j' && chaine[i] != 'K' && chaine[i] != 'k' && chaine[i] != 'L' && chaine[i] != 'l' &&
                                    chaine[i] != 'M' && chaine[i] != 'm' && chaine[i] != 'N' && chaine[i] != 'n' && chaine[i] != 'O' && chaine[i] != 'o' &&
                                    chaine[i] != 'P' && chaine[i] != 'p' && chaine[i] != 'Q' && chaine[i] != 'q' && chaine[i] != 'R' && chaine[i] != 'r' &&
                                    chaine[i] != 'S' && chaine[i] != 's' && chaine[i] != 'T' && chaine[i] != 't' && chaine[i] != 'U' && chaine[i] != 'u' &&
                                    chaine[i] != 'V' && chaine[i] != 'v' && chaine[i] != 'W' && chaine[i] != 'w' && chaine[i] != 'X' && chaine[i] != 'x' &&
                                    chaine[i] != 'Y' && chaine[i] != 'y' && chaine[i] != 'Z' && chaine[i] != 'z' && chaine[i] != '$' && chaine[i] != '%' &&
                                    chaine[i] != '*' && chaine[i] != '+' && chaine[i] != '-' && chaine[i] != '.' && chaine[i] != '/' && chaine[i] != ':' && chaine[i] != ' ')
                                {
                                    special = true;
                                }
                            }
                            if (special == true)
                            {
                                Console.WriteLine("Votre saisie comporte des caractères non autorisée. Veuillez réessayer.");
                                chaine = Console.ReadLine();
                            }
                            else
                            {
                                test = true;
                            }
                        }

                        //Déterminer la version du QR en fonction de la longueur de la chaine
                        int length = 0;      //va permettre de donner la version du QR en fonction de la taille de la chaine
                        if (chaine.Length <= 25)
                        {
                            length = 21;
                        }
                        else
                        {
                            length = 25;
                        }

                        //Génération du QR
                        MyImage image1 = new MyImage("./lac.bmp");      //création d'une image temporaitre qui va être écrasé pour placer le QR
                        image1.QRCode(chaine, length);
                        image1.From_Image_To_File("QR_Code");

                        Console.WriteLine("Votre QR Code a été enregistré avec succés sous le nom de QR_Code");
                    }
                    else                                             //Cas lecture QR
                    {
                        bool test = false;
                        MyImage QR = new MyImage("./lac.bmp");  //déclarer temporairement une image pour l'utilser après 
                                                                //teste si le fichier existe et si la taille correspond à un fichier de QR version 1 ou 2
                        while (test == false)
                        {
                            bool test1 = false;
                            Console.WriteLine("Saisir le nom du fichier contenant le QR sous son nom d'origine.");
                            string entree1 = Console.ReadLine();
                            string fichierQR = "./" + entree1 + ".bmp";
                            test1 = File.Exists(fichierQR);
                            if (test1 == true)
                            {
                                QR = new MyImage(fichierQR);
                                if ((QR.Width == 25 & QR.Height == 25) || (QR.Width == 21 & QR.Height == 21))
                                {
                                    test = true;
                                }
                            }

                        }

                        Console.WriteLine("Message contenu dans votre QR Code: " + QR.LectureQR(QR));   //afficher le message sur la console
                    }

                    //Demander à l'utilisateur si il souhaite recommancer.
                    Console.WriteLine("Souhaitez vous recommencer ? Si oui tappez 1 sinon tappez 0");
                    string recommencer = Console.ReadLine();

                    while (recommencer != "1" & recommencer != "0")
                    {
                        Console.WriteLine("Saisie incorrecte. Veuillez recommencer.");
                        recommencer = Console.ReadLine();
                    }

                    if (recommencer == "0") restart = true;
                }

            }
            if (action1 == "3")           //Création d'une fractale
            {
                MyImage i = new MyImage("./lac.bmp");
                MyImage fract = new MyImage(3000, 3000, i.Fractale());
                fract.From_Image_To_File("imageFractale");
                Console.WriteLine("Votre image a été modifé avec succés sous le nom 'imageFractale'.");
            }
        }
    }
}
