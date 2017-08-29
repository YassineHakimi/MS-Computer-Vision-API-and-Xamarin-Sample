using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOT.Models
{
 
        // This will be used to convert the response from json to a c# class
        // To create it, get the json response, copy it and then go to Edit>Paste Special>Paste Json as classes
        public class Rootobject
        {
            public string status { get; set; }
            public Recognitionresult recognitionResult { get; set; }
        }

        public class Recognitionresult
        {
            public Line[] lines { get; set; }
        }

        public class Line
        {
            public int[] boundingBox { get; set; }
            public string text { get; set; }
            public Word[] words { get; set; }
        }

        public class Word
        {
            public int[] boundingBox { get; set; }
            public string text { get; set; }
        }

}
