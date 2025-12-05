using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAGSystem.Models
{
    public class Chunk
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public float[] Vector { get; set; } = Array.Empty<float>();
        public double Similarity { get; set; }
    }
}