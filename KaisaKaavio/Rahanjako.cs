using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public class Rahanjako
    {
        public float OsallistumisMaksut = 0;
        public float KabikeMaksut = 0;
        public float LisenssiMaksut = 0;
        public float SeuranJasenMaksut = 0;
        public float RahaaYhteensa = 0;
        public int SbilOsuus = 0;
        public bool SbilOsuusOnProsentteja = false;
        public int SeuranOsuus = 0;
        public bool SeuranOsuusOnProsentteja = false;

        public float SbilOsuusSumma = 0;
        public float SeuranOsuusSumma = 0;
        public float Palkintoihin = 0;

        public int PalkittujenMaara = 3;
        public int VoittajanOsuus = 50;

        private List<Pelaaja> tulokset = new List<Pelaaja>();

        public void AlustaRahanjako(Kilpailu kilpailu, Loki loki)
        {
            try
            {
                this.OsallistumisMaksut = 0.0f;
                this.KabikeMaksut = 0.0f;
                this.LisenssiMaksut = 0.0f;
                this.SeuranJasenMaksut = 0.0f;
                this.RahaaYhteensa = 0.0f;

                foreach (var osallistuja in kilpailu.Osallistujat.Where(x => x.Id >= 0))
                {
                    float os = ParseFloat(osallistuja.OsMaksu);
                    float ka = ParseFloat(osallistuja.KabikeMaksu);
                    float li = ParseFloat(osallistuja.LisenssiMaksu);
                    float se = ParseFloat(osallistuja.SeuranJasenMaksu);

                    this.OsallistumisMaksut += os;
                    this.KabikeMaksut += ka;
                    this.LisenssiMaksut += li;
                    this.SeuranJasenMaksut += se;

                    this.RahaaYhteensa += os;
                    this.RahaaYhteensa += ka;
                    this.RahaaYhteensa += li;
                    this.RahaaYhteensa += se;
                }

                this.tulokset.Clear();
                if (kilpailu.Voittaja() != null)
                {
                    this.tulokset.AddRange(kilpailu.Tulokset());
                }
            }
            catch (Exception e)
            {
                loki.Kirjoita("Rahanjaon alustus epäonnistui", e, true);
            }
        }

        private float ParseFloat(string s)
        {
            float result = 0.0f;

            if (!string.IsNullOrEmpty(s))
            {
                StringBuilder sb = new StringBuilder();

                foreach (var c in s)
                {
                    if (Char.IsDigit(c) || c == '.' || c == ',')
                    {
                        sb.Append(c);
                    }
                }

                string input = sb.ToString();
                if (!string.IsNullOrEmpty(input))
                {
                    if (Single.TryParse(input, out result))
                    {
                        return result;
                    }

                    int asint = 0;
                    if (Int32.TryParse(input, out asint))
                    {
                        result = (float)asint;
                    }
                }
            }

            return result;
        }

        private float PyoristaYlospain(float f, int p)
        {
            if (f < 0.0f)
            {
                return 0.0f;
            }
            else
            {
                int i = (int)Math.Ceiling(f);
                while ((i % p) != 0)
                {
                    i++;
                }
                return (float)i;
            }
        }

        private float PyoristaYlospain(float f)
        {
            if (f < 10.0f)
            {
                return PyoristaYlospain(f, 1);
            }
            else if (f < 100.0f)
            {
                return PyoristaYlospain(f, 5);
            }
            else if (f < 250.0f)
            {
                return PyoristaYlospain(f, 10);
            }
            else if (f < 500.0f)
            {
                return PyoristaYlospain(f, 20);
            }
            else
            {
                return PyoristaYlospain(f, 50);
            }
        }

        private float PyoristaAlaspain(float f, int p)
        {
            if (f < 0.0f)
            {
                return 0.0f;
            }
            else
            {
                int i = (int)Math.Floor(f);
                while ((i % p) != 0)
                {
                    i--;
                }
                return (float)i;
            }
        }

        private float PyoristaAlaspain(float f)
        {
            if (f < 100.0f)
            {
                return PyoristaAlaspain(f, 5);
            }
            else if (f < 250.0f)
            {
                return PyoristaAlaspain(f, 10);
            }
            else if (f < 500.0f)
            {
                return PyoristaAlaspain(f, 20);
            }
            else
            {
                return PyoristaAlaspain(f, 50);
            }
        }

        public void PaivitaRahanjako(DataGridView taulukko, Loki loki)
        {
            try
            {
                float jaossa = this.OsallistumisMaksut;

                if (this.SbilOsuus > 0 && jaossa > 0.0f)
                {
                    if (this.SbilOsuusOnProsentteja)
                    {
                        this.SbilOsuusSumma = PyoristaYlospain(((float)this.SbilOsuus / 100.0f) * jaossa);
                    }
                    else 
                    {
                        this.SbilOsuusSumma = (float)this.SbilOsuus;
                    }

                    this.SbilOsuusSumma = Math.Min(jaossa, this.SbilOsuusSumma);
                    jaossa -= this.SbilOsuusSumma;
                    jaossa = Math.Max(0.0f, jaossa);
                }

                if (this.SeuranOsuus > 0 && jaossa > 0.0f)
                {
                    if (this.SeuranOsuusOnProsentteja)
                    {
                        this.SeuranOsuusSumma = PyoristaYlospain(((float)this.SeuranOsuus / 100.0f) * jaossa);
                    }
                    else
                    {
                        this.SeuranOsuusSumma = (float)this.SeuranOsuus;
                    }

                    this.SeuranOsuusSumma = Math.Min(jaossa, this.SeuranOsuusSumma);
                    jaossa -= this.SeuranOsuusSumma;
                    jaossa = Math.Max(0.0f, jaossa);                    
                }

                this.Palkintoihin = jaossa;

                taulukko.Rows.Clear();

                if (PalkittujenMaara > 0 && jaossa > 0.0f)
                {
                    float vipu = ((float)this.VoittajanOsuus) / 100.0f;

                    List<float> osuudet = new List<float>();
                    List<float> palkinnot = new List<float>();
                    float osuuksienSumma = 0.0f;
                    float palkintojenSumma = 0.0f;

                    for (int i = 0; i < PalkittujenMaara; ++i)
                    {
                        float d = 1.0f - (((float)i) / ((float)PalkittujenMaara));
                        float max = (float)Math.Pow(d, 1.0f / 0.5f);
                        float min = (float)Math.Pow(d, 1.0f / 10.0f);

                        float osuus = vipu * max + (1 - vipu) * min;

                        osuudet.Add(osuus);
                        osuuksienSumma += osuus;
                    }

                    if (osuuksienSumma > 0.0f)
                    {
                        for (int i = 0; i < PalkittujenMaara; ++i)
                        {
                            osuudet[i] /= osuuksienSumma;
                            float osuus = osuudet[i];
                            float palkinto = PyoristaYlospain(osuus * jaossa);
                            palkinnot.Add(palkinto);
                            palkintojenSumma += palkinto;
                        }

                        // Varmistetaan että palkintojen summa täsmää jaettavaan määrään
                        if (palkintojenSumma > jaossa)
                        {
                            palkinnot[palkinnot.Count - 1] -= (palkintojenSumma - jaossa);
                        }
                        else if (palkintojenSumma < jaossa)
                        {
                            palkinnot[0] += (jaossa - palkintojenSumma);
                        }

                        // Varmistetaan että kaikki saa jotain
                        for (int i = PalkittujenMaara - 2; i >= 0; --i)
                        {
                            if (palkinnot[i + 1] < 5)
                            {
                                float summa = 5 - palkinnot[i + 1];
                                palkinnot[i + 1] += summa;
                                palkinnot[i] -= summa;
                            }
                        }

                        for (int i = 0; i < PalkittujenMaara; ++i)
                        {
                            float palkinto = palkinnot[i];

                            taulukko.Rows.Add(
                                (i + 1).ToString(),
                                i < this.tulokset.Count ? this.tulokset[i].Nimi : string.Empty,
                                ((int)palkinto).ToString() + " €",
                                ((int)(osuudet[i] * 100.0f)).ToString() + "%");

                        }
                    }
                }
            }
            catch (Exception e)
            {
                loki.Kirjoita("Rahanjaon päivitys epäonnistui", e, true);
            }
        }
    }
}
