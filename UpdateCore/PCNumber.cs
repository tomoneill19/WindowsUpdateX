using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateCore
{
    public class PCNumber
    {
        //Properties
        double number;
        string placeholderText = "";
        int placeholder = 0;

        public PCNumber(double number)
        {
            this.number = number;
            parse();
        }

        private void parse() // Parse a number of bits into its order of magnitude (E.G. KB, MB etc.)
        {
            do
            {
                number /= 1024;
                placeholder += 1;
            } while (number > 1024);

            switch (placeholder)
            {
                case 0:
                    placeholderText = "B";
                    break;
                case 1:
                    placeholderText = "K";
                    break;
                case 2:
                    placeholderText = "M";
                    break;
                case 3:
                    placeholderText = "G";
                    break;
                case 4:
                    placeholderText = "T";
                    break;
            }
        }
        public string getPlaceHolderText()  //Returns the text associated with the order of magnitude of a number of bits (E.G. KB, MB etc.)
        {
            return placeholderText;
        }

        public double getNumber() // gets the number part of the parsed value
        {
            return number;
        }

        public string getDisplayNumber() //gets the combined string of the number and the place value text.
        {
            return Math.Round(getNumber(), 0) + " " + getPlaceHolderText(); // The value is rounded to the nearest integer
        }

    }
}