//loads sounds
//edit: for some unknown reason, the index of the arrays must be set to nine (total 10 items) when there is only 9!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Soundboard
{
    public class LoadData
    {
        //all the buttons and their file destination. 
        private static string[] Q = new string[9]; // e.g. Q[0] is "1" button on the q series
        private static string[] A = new string[9];
        private static string[] Z = new string[9];
        private static string[] W = new string[9];
        private static string[] S = new string[9];

        public static bool saved = false;
        

        public LoadData()
        {

        }



        public LoadData(string[] _q, string[] _a, string[] _z, string[] _w, string[] _s)
        {
            // for (int i = 0; i < _q.Length; i++)
            //{
            //  Q[i] = _q[i];
            //A[i] = _a[i];
            //Z[i] = _z[i];
            //W[i] = _w[i];
            //S[i] = _s[i];
            //}

            Array.Copy(_q, Q, 9);
            Array.Copy(_a, A, 9);
            Array.Copy(_z, Z, 9);
            Array.Copy(_w, W, 9);
            Array.Copy(_s, S, 9);
    
            saved = true;
            return;
 
        }

        public static void write()
        {
            Thread.Sleep(100);
            
            Form1.ReadDATA(Q, A, Z, W, S);
        }
        //public Array Setdata(string[] Qloc, string[] Aloc, string[] Zloc, string[] Wloc, string[] Sloc)
        //{
           // Array.Copy(Q, Qloc, 9);
           // Array.Copy(A, Aloc, 9);
           // Array.Copy(Z, Zloc, 9);
           // Array.Copy(W, Wloc, 9);
           // Array.Copy(S, Sloc, 9);

            //return Qloc;
        //}
        //add a save function to save these locations to the computer.
        //also check for errors in case the file doesnt exist anymore.
    }
}
