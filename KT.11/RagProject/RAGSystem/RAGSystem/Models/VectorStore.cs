using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RAGSystem.Models
{
    public class VectorStore
    {
        public List<Chunk> Chunks { get; set; } = new List<Chunk>();
    }
}