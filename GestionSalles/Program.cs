using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;


internal class Program
{
    static void Main(string[] args)
    {
        //int year = 0;

        //do
        //{
        //    Console.Write("L'année ");
        //    year = Convert.ToInt32(Console.ReadLine());
        //    int y = (int)(year * 0.01);
        //    //Console.WriteLine(year);
        //    if (y * 100 != year) { y = year; }
        //    Console.Write(" est ");
        //    if (!(y % 4 == 0)) { Console.Write("non "); }
        //    Console.WriteLine("bissextile.");
        //    Console.WriteLine("");
        //}
        //while (year != 0);

        //Initialisation des données

        InitialisationCSVAleatoire();

        Console.Clear();

        //Analyse des données
        Console.WriteLine("Nom du fichier CSV à analyser : ");
        string fichierCSV = Console.ReadLine().ToLower();

        int[,] TableauOccupation = LectureCSV(fichierCSV); //on lit le fichier et on le convertit en tableau
        //ShowGrid(TableauOccupation); // affichage du tableau
        
        Double[] StatOccupationParSemaine = new Double[52];
        StatOccupationParSemaine = CalculStat(TableauOccupation);

        //int choix=0;
        //do
        //{
        //    Console.Clear();
        //    Console.WriteLine("Quitter : 0");
        //    Console.WriteLine("Affichage de la grille générale : 1");
        //    Console.WriteLine("Affichage des taux d'occupation par semaine sur l'année : 2");

            Console.WriteLine("Taux occupation par semaine :");

            for (int i = 0; i < 52; i++)
            {
                Console.Write("Semaine " + (i + 1) + (char)9 + " : ");
                if (StatOccupationParSemaine[i] < 0)
                {
                    Console.WriteLine("Pas d'occupation");
                }
                else
                {
                    Console.WriteLine((StatOccupationParSemaine[i]) + "%");
                }
            }
        //}
        //while (choix !=0);
    }

    static Double[] CalculStat(int[,] T)
    {
        int nbSem = T.GetLength(0);
        int nbSalles = T.GetLength(1);
        Double[] Stat = new Double[nbSem];  
        int somme = 0;
        int i, j;

        for (i = 0; i < nbSem; i++)
        {
            for (j = 0; j < nbSalles; j++)
            {
                somme += T[i, j];
            }
            Stat[i] = 100 * somme / nbSalles;
            somme = 0;
        }
        return Stat;
    }
    static int[,] LectureCSV(string fichierCSV)
    {
        
        int i = 0;
        int j=0;
        int[,] T = new int[52,26]; // tableau taille max qui servira à placer les données du CSV de taille inconnue
        int character, buf;
        using (StreamReader sw = new StreamReader(fichierCSV + ".csv"))
        {
            // lecture de la première ligne qui ne contient que les noms de salles
            do
            {
                character = sw.Read();
            }
            while (character != 10);
            
            //lecture par caractère du fichier CSV
            while (!sw.EndOfStream)
            {
                character = sw.Read();

                switch (character)
                {
                    case 10: //si LF (char 10) alors changement de ligne, 
                        j++; // on incrémente j
                        i = 0; // on remet à zéro le numéro de colonne
                        break;
                        
                    case 45:
                        T[i, j] = -1;
                        buf = sw.Read(); //case 45: repère le - de -1, une lecture permet de passer le 1
                        i++;
                        break;
                    case 48: // char 48 : 0
                        T[i, j] = 0;
                        i++;
                        break;
                    case 49: // char 49 : 1
                        T[i,j]=1;
                        i++;
                        break;
                 }
            }
        }
        
        // copie du tableau taille max dans un tableau de la bonne grandeur
        int[,] TS = new int[52, j+1];
        for (i = 0; i < 52; i++)
        {
            for (int k = 0; k < j+1; k++)
            {
                TS[i, k] = T[i, k];
            }
        }
        return TS; // on retourne le tableau
    }

    static void InitialisationCSVAleatoire()
    {
        int[,] TableauOccupation;
        List<int> semainesfermees = new List<int>() { };
        int semaine = 0;
        int nbsalles = 0;
        string nomfichierCSV;

        //Combien de salles ?
        do
        {
            Console.Clear();
            Console.WriteLine("Entrez le nombre de salles (entre 1 et 26) : ");
            nbsalles = Convert.ToInt32(Console.ReadLine()); // on stocke le nombre dans une variable
        }
        while (!(nbsalles >= 1 && nbsalles <= 26));

        //Quelles semaines de fermetures ? 0 pour sortir de la boucle
        do
        {
            semaine = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("Entrez un numéro de semaine où le centre est fermé (0 pour sortir) : ");
                semaine = Convert.ToInt32(Console.ReadLine());
            }
            while (!(semaine >= 0 && semaine <= 52));
            semainesfermees.Add(semaine); //on stocke les semaines dans une liste
        }
        while (semaine != 0); //sortie de la boucle si semaine =0


        // nom du fichier .csv de stockage
        Console.WriteLine("Nom du fichier de données CSV : ");
        nomfichierCSV = Console.ReadLine().ToLower();


        //création aléatoire des occupations de salles tenant compte des semaines fermées
        TableauOccupation = TableauAleatoire(nbsalles, semainesfermees);

        // on convertit le tableau aléatoire en chaine au format CSV qu'on écrit dans le fichier
        using (StreamWriter sw = new StreamWriter(nomfichierCSV+".csv"))
        {
            sw.Write(ConvertTabtoCSV(TableauOccupation));
        }

    }

    public static int[,] TableauAleatoire(int nbsalles, List<int> semainesfermees)
    {
        Random rand = new Random();
        
        int[,] T = new int[52, nbsalles];
        int coef = rand.Next(101) ;
        
        for (int i=0; i<52; i++)
        {
            double randomProba = rand.NextDouble(); // probabilité de générer 1

            for (int j=0; j < nbsalles; j++)
            {
                if (semainesfermees.Contains(i+1) ) //si le centre est fermé
                { 
                    T[i,j] = -1; // on place -1 dans le tableau
                }
                else                  //sinon
                {
                    double randomNb = rand.NextDouble();
                    int result = (randomNb < randomProba) ? 1 : 0;  //si le nombre est inférieur à la proba alors on place 1, sinon 0
                    T[i,j] = result;  //on écrit le résultat dans le tableau (1: occupé; 0: libre)
                }
            }
        }
        return T;
    }
    static string ConvertTabtoCSV(int[,] T)
    {
        int la, ht, i, j;
        ht = T.GetLength(0);
        la = T.GetLength(1);
        string ch = "";

        //ligne de titre avec les numéro de semaine
        for (i=0; i < ht; i++)
        {
            ch +=  ",Sem" + (i + 1);

        }
        ch += (char)10; // changement de ligne
        for (j=0; j<la; j++)
        {
            ch += "Salle" + (Char)(j + 65); // première colone de chaque ligne avec le nom de la salle
            for (i=0; i<ht; i++)
            {
                ch += ","+T[i, j]; // chaque donnée du tableau est ajouté, précédée d'une virgule de séparation
            }
            if (j != la - 1) { ch += (char)10; }; //changement de ligne
        }
        // Console.WriteLine(ch); //écriture de la chaine pour controle
        return ch; // on retourne la chaine
    }


    static void ShowGrid(int[,] T1) //Fonction affichant la grille de jeu
    {
        int i, j, la, ht, value;
        //Console.Clear();
        string spc = "  ";
        la = T1.GetLength(1);
        ht = T1.GetLength(0);
        string[,] T = new string[ht, la];

        Console.WriteLine("LEGENDE :  ■ : centre fermé   /   O : salle occupée   /   - : salle vide"); //légende
        Console.WriteLine("");

        // Conversion de la grille d'entiers en grille de caractères pour l'affichage
        for (j = 0; j < la; j++)
        {
            for (i = 0; i < ht; i++)
            {
                value = T1[i, j];
                switch (value)
                {
                    case 0:
                        {
                            T[i, j] = "-";
                            break;
                        }
                    case -1:
                        {
                            T[i, j] = "■";
                            break;
                        }
                    case 1:
                        {
                            T[i, j] = "O";
                            break;
                        }
                    default:
                        {
                            T[i, j] = "T"; 
                            break;
                        }
                }
            }
        }

        //AFFICHAGE
        //Affichage ligne A B C D ,etc jusqu'à Z
        Console.Write("  ");
        for (j = 0; j < la; j++)
        {
            Console.Write("  " + (char)(65 + j) + " ");
        }
        Console.WriteLine("");

        //Affichage première ligne tableau +---+---+--- etc
        Console.Write("  ");
        for (j = 0; j < la; j++)
        {
            Console.Write("+---");
        }
        Console.WriteLine("+"); //fermeture de la ligne

        //Affichage lignes courantes + ligne finale avec valeurs tableau
        for (i = 0; i < ht; i++)
        {
            if (i < 9) { spc = " "; } else { spc = ""; };
            //ligne avec valeur
            Console.Write((i + 1) + spc);
            for (j = 0; j < la; j++)
            {
                Console.Write("| " + T[i, j] + " ");
            }
            Console.WriteLine("|");
            //ligne de séparation horizontale
            Console.Write("  ");
            for (j = 0; j < la; j++)
            {
                Console.Write("+---");
            }
            Console.WriteLine("+");
        }
        Console.WriteLine("LEGENDE :  ■ : centre fermé   /   O : salle occupée   /   - : salle vide"); // répétition de la légende

    }
}