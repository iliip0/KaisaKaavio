using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Integraatio
{
    public class BiljardiOrgKilpailu
    {
        public string Id { get; set; }
        public string Nimi { get; set; }

        public BiljardiOrgKilpailu()
        {
            this.Id = string.Empty;
            this.Nimi = string.Empty;
        }

        public string ToString()
        {
            return this.Nimi;
        }
    }
}
