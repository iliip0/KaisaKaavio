using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private Kilpailu kilpailu = null;
        private List<Pelaaja.TulosTietue> tulokset = new List<Pelaaja.TulosTietue>();

        public void AlustaRahanjako(Kilpailu kilpailu, Loki loki)
        {
            this.kilpailu = kilpailu;

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
                if (kilpailu.KilpailuOnPaattynyt)
                {
                    this.tulokset.AddRange(kilpailu.Tulokset());
                }
                else
                {
                    int sijoitus = 1;
                    foreach (var o in kilpailu.Osallistujat)
                    {
                        Pelaaja.TulosTietue tulos = new Pelaaja.TulosTietue() 
                        {
                            Sijoitus = sijoitus++ 
                        };

                        this.tulokset.Add(tulos);
                    }
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

                jaossa = PaivitaSbilOsuus(jaossa);
                jaossa = PaivitaSeuranOsuus(jaossa);

                this.Palkintoihin = jaossa;

                taulukko.Rows.Clear();

                int palkittujenMaara = 0;
                int edellinenSijoitus = 0;
                int vikallaSijalla = 1;

                foreach (var tulos in this.tulokset)
                {
                    if (palkittujenMaara < this.PalkittujenMaara)
                    {
                        edellinenSijoitus = tulos.Sijoitus;
                        palkittujenMaara++;
                    }
                    else
                    {
                        if (tulos.Sijoitus == edellinenSijoitus)
                        {
                            palkittujenMaara++;
                            vikallaSijalla++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                palkittujenMaara = Math.Min(palkittujenMaara, this.tulokset.Count);

                if (palkittujenMaara > 0 && jaossa > 0.0f)
                {
                    float vipu = ((float)this.VoittajanOsuus) / 100.0f;

                    List<float> osuudet = new List<float>();
                    List<float> palkinnot = new List<float>();
                    float osuuksienSumma = 0.0f;
                    float palkintojenSumma = 0.0f;

                    for (int i = 0; i < palkittujenMaara; ++i)
                    {
                        float d = 1.0f - (((float)i) / ((float)palkittujenMaara));
                        float max = (float)Math.Pow(d, 1.0f / 0.5f);
                        float min = (float)Math.Pow(d, 1.0f / 10.0f);

                        float osuus = vipu * max + (1 - vipu) * min;

                        osuudet.Add(osuus);
                        osuuksienSumma += osuus;
                    }

                    if (osuuksienSumma > 0.0f)
                    {
                        Dictionary<int, List<float>> osuudetSijoittain = new Dictionary<int, List<float>>();

                        for (int i = 0; i < palkittujenMaara; ++i)
                        {
                            osuudet[i] /= osuuksienSumma;

                            int sijoitus = this.tulokset[i].Sijoitus;
                            if (!osuudetSijoittain.ContainsKey(sijoitus))
                            {
                                osuudetSijoittain[sijoitus] = new List<float>();
                            }

                            osuudetSijoittain[sijoitus].Add(osuudet[i]);
                        }

                        Dictionary<int, float> osuusSijoittain = new Dictionary<int, float>();

                        // Tasataan osuudet jos pelaajia on tasapisteissä palkinnoilla
                        foreach (var o in osuudetSijoittain)
                        {
                            osuusSijoittain.Add(o.Key, o.Value.Average());
                        }

                        for (int i = 0; i < palkittujenMaara; ++i)
                        {
                            float osuus = osuusSijoittain[this.tulokset[i].Sijoitus];
                            osuudet[i] = osuus;
                            float palkinto = 0.0f;

                            if (osuudetSijoittain[this.tulokset[i].Sijoitus].Count > 1)
                            {
                                palkinto = (float)Math.Ceiling((double)(osuus * jaossa));
                            }
                            else
                            { 
                                palkinto = PyoristaYlospain(osuus * jaossa);
                            }

                            palkinnot.Add(palkinto);
                            palkintojenSumma += palkinto;
                        }

                        // Varmistetaan että palkintojen summa täsmää jaettavaan määrään
                        if (palkintojenSumma > jaossa)
                        {
                            if (vikallaSijalla > 1)
                            {
                                float kokoOsuus = palkintojenSumma - jaossa;
                                float jakoOsuus = (float)Math.Floor((double)(kokoOsuus / vikallaSijalla));

                                for (int i = 0; i < vikallaSijalla; ++i)
                                {
                                    palkinnot[palkinnot.Count - 1 - i] -= jakoOsuus;
                                    kokoOsuus -= jakoOsuus;
                                }

                                if (kokoOsuus > 0.0f)
                                {
                                    palkinnot[palkinnot.Count - 1] -= kokoOsuus;
                                }
                            }
                            else
                            {
                                palkinnot[palkinnot.Count - 1] -= (palkintojenSumma - jaossa);
                            }
                        }
                        else if (palkintojenSumma < jaossa)
                        {
                            palkinnot[0] += (jaossa - palkintojenSumma);
                        }

                        // Varmistetaan että kaikki saa jotain
                        for (int i = palkittujenMaara - 2; i >= 0; --i)
                        {
                            if (palkinnot[i + 1] < 1)
                            {
                                float summa = 1 - palkinnot[i + 1];
                                palkinnot[i + 1] += summa;
                                palkinnot[i] -= summa;
                            }
                        }

                        for (int i = 0; i < palkittujenMaara; ++i)
                        {
                            float palkinto = palkinnot[i];

                            taulukko.Rows.Add(
                                i < this.tulokset.Count ? this.tulokset[i].Sijoitus.ToString() : (i + 1).ToString(),
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

        private float PaivitaSeuranOsuus(float jaossa)
        {
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
            return jaossa;
        }

        private float PaivitaSbilOsuus(float jaossa)
        {
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
            return jaossa;
        }
    }
}
