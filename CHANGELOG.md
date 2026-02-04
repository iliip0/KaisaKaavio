# KaisaKaavio ohjelman versiohistoria

Tämä dokumentti sisältää ytimekkään kuvauksen mitä muutoksia ohjelman versiot sisältävät verrattuna aiempiin versioihin,

## Versio 1.0.0.47		(julkaistu 4.2.2026)

* Lisätty kommentti "Haettu käsin" käsin haettuihin peleihin 'Tulokset'-osiossa
* Lisätty uusi "Hae pelejä käsin" nappi 'Kaavio'-osioon
* Käsin haku mahdolliseksi myös joukkuekilpailussa

## Versio 1.0.0.46		(julkaistu 5.12.2025)

* Hakualgoritmin optimointia
* Poistettu "Rahanjako" tab kun pelataan SBiL alaisia SM tai RG kilpailuja
* Poistettu "Osallistumismaksu" ja "Veloitettu" sarakkeet pelaajalistalta kun pelataan SBiL alaisia SM tai RG kilpailuja
* Lisätty kaaviotyypit "pudotuspelit 5. kierroksesta alkaen" ja "pudotuspelit 6. kierroksesta alkaen"

## Versio 1.0.0.45		(julkaistu 27.11.2025)

* Korjaus kilpailijoiden hakuun biljardi.org sivulta. Saattoi epäonnistua SSL tarkistukseen aiemmin
* Korjaus hakualgoritmiin, jotta ei jätä kierroksen perälle kahta jo keskenään pelannutta

## Versio 1.0.0.44		(julkaistu 20.11.2025)

* Online ilmoittautuminen
* Online viikkokisaranking
* Lisätty mahdollisuus luoda nimetty rankingsarja, johon voi laittaa mitä vaan viikkokisoja
* Lisätty peliparien arvontaominaisuus joukkuekilpailuun
* Bugikorjaus ottelupöytäkirjojen tulostukseen

## Versio 1.0.0.43		(julkaistu 14.10.2025)

* Korjaus ilmoittautumissivun automaattiseen nimien ehdotukseen
* Alustava integraation KaisaKaavio.fi sivuston kanssa

## Versio 1.0.0.42		(julkaistu 15.09.2025)

* Korjauksia ja parannuksia hakualgoritmiin
* Korjattu hakubugi usean pelipaikan kilpailuissa. Aiempi versio ei osannut hakea viimeistä usean pelipaikan kierrosta oikein

## Versio 1.0.0.41		(julkaistu 04.09.2025)

* Korjattu hakuvirheet, jotka johtuivat bugista versioissa 1.0.0.37-1.0.0.40

## Versio 1.0.0.40		(julkaistu 03.09.2025)

* Lisätty kaaviotyyppi "pudotuspelit 4. kierroksesta alkaen"
* Yhdistetty kaikki ohjelmaversiot (KaisaKaavio, KaisaKaavioLite ja KaisaKaavioMulti) yhdeksi ohjelmaksi (KaisaKaavio.exe). Tämä on virustorjuntaystävällinen, ja sallii avata useita kilpailuja samalla koneella.

## Versio 1.0.0.39		(julkaistu 30.8.2025)

* Lisätty erikoistapaus hakuun kun on enää kolme pelaajaa jäljellä. Tällöin voidaan hypätä tarvittaessa hakuvuorossa olevan yli
* Bugikorjaus, kaatui klikattaessa "Ohjelman tiedot" painiketta (regressiobugi noin versiosta 1.0.0.37)
* Bugikorjaus, estettiin turhien hakujen tekeminen tyhjien hakujen jälkeen

## Versio 1.0.0.38		(julkaistu 22.8.2025)

* Bugikorjauksia kun kaavio on "pudarit 2.kierroksesta alkaen"

## Versio 1.0.0.37		(julkaistu 11.8.2025)

* Parannettu hakua, kun jäljellä on 5 tai vähemmän pelaajia
* Laitettu lisenssi ja versiohistoria avautumaan ohjelman sisäiseen ikkunaan
* Korjattu ottelupöytäkirjatuloste mahtumaan paremmin sivulle

## Versio 1.0.0.36		(julkaistu 18.6.2025)

* Lisätty KaisaKaavioLite.exe versio ohjelmasta, joka on virustorjunta-ystävällisempi. Tämä versio ei päivity automaattisesti uusimpaan versioon

## Versio 1.0.0.35		(julkaistu 5.6.2025)

* Korjaus hakualgoritmiin. Algoritmi jätti joitain "varmoja" hakuja hakematta kun ei osannut huomioida pelejä joissa toinen pelaaja putoaa väkisin

## Versio 1.0.0.34		(julkaistu 2.5.2025)

* Mahdollistettu "kaavioiden yhdistäminen kierroksesta X alkaen" valinnan muuttaminen ensimmäisen ja toisen kierroksen aikana
* Lisätty mahdollisuus arpoa kaavio uudelleen valikon kautta
* Lisätty mahdollisuus vaihtaa kaaviotyyppi kisan aikana valikon kautta
* Lisätty mahdollisuus vaihtaa kaavioiden yhdistämiskierros kisan aikana valikon kautta usean pelipaikan kilpailuissa

## Versio 1.0.0.33		(julkaistu 30.04.2025)

* Korjattu bugi jossa usean pelipaikan kisassa pystyi alkukierroksilla hakemaan manuaalisesti pelin eri pelipaikkojen pelaajien välillä 
* Näppäimillä navigointi "lyöntivuoroja" sarakkeeseen mahdolliseksi karakisoissa
* Korjattu 24 pelaajan sijoittaminen kaavioon

## Versio 1.0.0.32		(julkaistu 29.4.2025)

* Striimilinkit korjattu "Tulokset" osioon
* Korjattu bugi jossa pelin pöytä muuttuu kun pelin pisimpiä sarjoja syötetään
* Lyöntivuorojen määrän editointi lisätty "Pelin tiedot" popuppiin
* Mahdollisuus lisätä (DNS) pelaajalle "Pelin tiedot" popupissa
* Ottelupöytäkirjojen tulostus korjattu niin että pöytäkirjat tulostuvat kokonaisina

## Versio 1.0.0.31		(julkaistu 9.4.2025)

* Lisätty KaisaKaavioMulti.exe, jolla pystyy pelaamaan useita kaavioita samalla koneella samanaikaisesti

## Versio 1.0.0.30		(julkaistu 4.4.2025)

* Lisätty tarkistuksia uutta kilpailua luotaessa, jotta käyttäjä ei pysty luomaan kilpailuja ei-tuetulla formaatilla
* Korjattu tulostettavien pelipöytäkirjojen muotoilu parikisassa
* Korjauksia joukkuekilpailun ajoon
* Muutettu kisojen oletuskansiot menemään "Documents/KaisaKaaviot/" kansion alle kaikki

## Versio 1.0.0.29		(julkaistu 2.4.2025)

* Lisätty mahdollisuus vetää Kaisan joukkue SM-kisoja
* Lisätty mahdollisuus valita kilpailun kansio kilpailua luotaessa
* Laitettu kilpailut tallentumaan oletusarvoisesti laji- ja kilpailun tyyppikohtaisiin omiin kansioihinsa

## Versio 1.0.0.28		(julkaistu 31.3.2025)

* Lisätty pelipöytäkirjojen tulostusmahdollisuus

## Versio 1.0.0.27		(julkaistu 7.3.2025)

* Kisakutsu välilehti näkyviin myös viikkokisamoodissa

## Versio 1.0.0.26		(julkaistu 7.3.2025)

* Lisätty paras pelikohtainen pistekeskiarvo karakilpailujen tulososioon
* Parannuksia ilmoittautuneiden hakuun biljardi.org sivulta
* Kaisan SM Parikilpailujen totetutus lisätty
* Kaisan joukkue SM kilpailujen toteutus aloitettu
* Palautettu pelit alkamaan aina 0-0 tilanteesta, vaikka olisi tasoituksia. Aiheutti sekaannusta
* Estetty kolmannen kierroksen pelien haku jos yhtään toisen kierroksen peliä ei ole pelattu
 
## Versio 1.0.0.25		(julkaistu 21.2.2025)

* Lisätty lyöntivuorojen määrän merkkaus Karakilpailuihin
* Lisätty pistekeskiarvo Karakilpailuihin

## Versio 1.0.0.24		(julkaistu 21.2.2025)

* Lisätty 24 pelaajan sijoittaminen Kaisan SM kilpailuihin
* Täydennetty kisojen kisakutsuinfoja kaisan ohjekirjan mukaisiksi

## Versio 1.0.0.23		(julkaistu 20.2.2025)

* Varsinaisen pelisalin tiedot muokattaviksi 'Useamman pelipaikan kilpailu' välilehdelle
* Ilmoittautuneiden haku biljardi.org sivulta - ominaisuuden teko aloitettu
* Kaisaa pelatessa tuloksissa pisteet pakotettu välille 0-60
* Lisätty mahdollisuus hakea ilmoittautuneet biljardi.org sivustolta (vain Kaisan RG ja SM kisoihin)

## Versio 1.0.0.22		(julkaistu 18.2.2025)

* Pisimmät sarjat lisätty pelien perään 'Pelit ja Tulokset' osiossa

## Versio 1.0.0.21		(julkaistu 17.2.2025)

* Lisätty mahdollisuus pelata alkukierrokset usealla pelipaikalla

## Versio 1.0.0.20		(julkaistu 2.2.2025)

* Lisätty "palauta oletuspisteytys" nappi ranking pisteytyksen editointi-ikkunaan
* Lisätty mahdollisuus Monte Carlo testata kaaviota generoimalla satunnaisia kisoja

## Versio 1.0.0.19		(julkaistu 30.1.2025)

* Bugikorjaus ranking pisteytyksiin, ei osannut laskea oikein kun ranking ykkönen, kakkonen tai kolmonen ei oo kisoissa mukana
* Bugikorjaus tulosten näyttämiseen kun kisa on kesken. Saattoi näyttää 3.sijan väärin
* Korjattu jaetut sijat pois tulosten alkupäästä kun kilpailu on vielä kesken

## Versio 1.0.0.18		(julkaistu 29.1.2025)

* Lisätty testikilpailua pelatessa mahdollisuus lisätä nappia painamalla 5, 10 tai 20 pelaajaa kaavioon
* Lisätty (CUP) teksti kierroksen perään pelit näkymässä jos peli on pudari
* Muutettu rankingpisteytyksiä niin, että pisteytykset tallentuvat kullekin sarjalle erikseen. Siten pisteytyksen muuttaminen jatkossa ei muuta vanhojen sarjojen pisteytystä
* Muutettu oletusrankingpisteytys tasaisemmaksi (vähemmän osakilpailun voittajaa suosivaksi)
* Bugikorjaus, avattu kilpailu ei mennyt viimeisimpiin kilpailuihin ikkunaa suljettaessa

## Versio 1.0.0.17		(julkaistu 19.1.2025)

* Uudet kuvat aloitussivun painikkeisiin.
* Aloitussivun parantelua

## Versio 1.0.0.16		(julkaistu 17.1.2025)

* Uusi aloitussivu, josta on helppo aloittaa uusi kilpailu.
* Mahdollisuus pelata "testikisoja", jotka tallentuvat eri paikkaan eri rankingeillä
* Korjauksia ja parannuksia SBiL keskustelupalsta tulosteisiin
* Korjauksia viikkokisaranking systeemiin
* Vanhojen loki ja varmuuskopioden automaattinen poisto

## Versio 1.0.0.15		(julkaistu 14.1.2025)

* Lisätty 'Tasuri' sarake ilmoittautumislistalle
* Vaihdettu niin että tulokset näytetään ennen pelejä SBiL osiossa jos kilpailu on päättynyt
* Asetettiin ohjelma aukeamaan tyhjänä. Aiemmin ohjelma avasi edellisen kisan alussa
* Osallistumismaksu automaattisesti pelaajille ilmoittautumissivulle

## Versio 1.0.0.14		(julkaistu 11.1.2025)

* Muutettu kilpailun tyyppivalintoja niin että valittavissa on 4 kisatyyppiä: { Viikkokisa, Avoin kilpailu, SBiL RG kilpailu ja SBiL SM kilpailu }
* Lisätty kilpailuun kilpasarja valittavaksi { Yleinen, SenioritMN40, SenioritMN50 jne... }
* Korjaus Salin Tiedot osioon. Osa tiedoista ei tallentunut
* Korjaus Rankingsarja ominaisuuteen: Pelaajien nimissä olevat tasurit yms merkinnät sotkivat taulukon. Korjattu
* Korjaus Rankingsarja ominaisuuteen: Rankingtilanne ei päivittynyt jos heti kisan päätyttyä mentiin 'Tulokset' osioon. Korjattu

## Versio 1.0.0.13		(julkaistu 9.1.2025)

* Mahdolliseksi käynnnissä olevan pelin peruuttaminen ei käynnissäolevaksi painamalla 'Pöytä X' nappia pelit taulukossa
* Bugikorjaus, Kaavio-näkymässä ei ollut voittoja, tappioita eikä pisteitä lopussa

## Versio 1.0.0.12		(julkaistu 8.1.2025)

* Lisätty 'viimeisimmät kilpailut' menuitem ylävalikkoon
* SBiL tulosteiden muotoilun parantelua

## Versio 1.0.0.11		(julkaistu 7.1.2025)

* Mahdollisuus muokata jo pelattujen pelien tietoja painamalla 'Pelattu' nappia 'Pelit' sivulla. Pelin tuloksen muuttaminen mitätöi myöhempiä pelejä tarvittaessa
* Rankingtilanne lisätty 'Tulokset' Sbil tekstin perään kun kisa on viikkokisa Rankingkisa
* Lisävaihtoehtoja ranking pisteiden määräytymiseen

## Versio 1.0.0.10		(julkaistu 3.1.2025)

* Kisainfo-välilehden siistiminen & kuvakkeiden kaunistus
* Päivämäärä lisätty 'Luo uusi peli' ikkunaan valittavaksi
* Päivämäärien esitys vaihdettu muotoon 25.2.2024
* Rankingkisa ominaisuuteen mahdolliseksi muuttaa kaikkia rankingiin liittyviä tietoja sekä lisätä/poistaa kilpailuja sarjasta jälkikäteen
* Kilpailun nimi lisätty SBiL keskustelupalstatulosteiden (Pelit/Tulokset) alkuun

## Versio 1.0.0.9		(julkaistu 30.12.2024)

* Pudonneiden pelaajien nimet paksulla fontilla tulokset osioon
* Pelien kestot lisätty 'Tulokset' osioon
* Pelien keskimääräinen kesto ja mediaanikesto lisätty tulokset osioon
* Korjattu bugi kun ekoille peleille laitetaan pöytä valmiiksi kisakutsuun. Kutsussa näkyy tällöin kisan alkamisaika
* Lisätty debug/testaustoiminto jossa vanhan kisan voi pelata peli kerrallaan uudelleen läpi, samassa ottelujärjestyksessä
* Korjattu bugi jossa 'Tulokset' osiossa pelaajien lopullinen sijoitus saattoi muuttua vielä viimeisten pelien aikana
* Lisätty varoituspopup jos käyttäjä yrittää muokata pelejä kun yhdessä pelissä on virheellinen tulos

## Versio 1.0.0.8		(julkaistu 26.12.2024)

* Lisätty automaattinen pöydän valinta kun painetaan 'Käynnistä peli'-nappia
* Lisätty vapaiden pöytien näyttö kun pöytää valitaan manuaalisesti uudelle pelille

## Versio 1.0.0.7		(julkaistu 20.12.2024)

* Ensimmäinen versio käyttöoppaasta valmis

## Versio 1.0.0.6		(julkaistu 15.12.2024)

* Korjaus Rahanjako välilehdelle: summien tasaus kun useampi pelaaja on tasapisteissä
* Kisainfoon valittavaksi kilpailun sijoitusten määräytymisperuste (montako parasta otetaan pisimmälle päässeistä voittojen ja pisteiden sijaan)
* Lisätty kuvakkeita valikoihin sekä muuta pientä koristelua
* Ranking-välilehti nätimmäksi

## Versio 1.0.0.5		(julkaistu 13.12.2024)

* Korjaus hakualgoritmiin. Automaattinen testiajo löysi bugin
* Pelien hakeminen manuaalisesti Kaavio-välilehdeltä toteutettu. Tämä mahdollistaa täyden kontrollin kilpailun vetäjälle hakea pelejä tai korjata mahdollisia hakuvirheitä
* Lisätty mitalikuvat Kaavio-välilehdelle kilpailun päätyttyä

## Versio 1.0.0.4		(julkaistu 10.12.2024)

* Pelaajien sijoittaminen SBiL RG kisoihin toteutettu
* Automaattinen nimien ehdotus ilmoittautuneet listalle, sekä jälki-ilmoittautuneet listalle
* Teksti (finaali) pelit tekstitulosteeseen finaalikierroksille
* Kaksi testikilpailua lisätty ohjelman testaamiseen. Testikaavioita 13 tässä versiossa

## Versio 1.0.0.3		(julkaistu 9.12.2024)

* Tuki viikokilpailujen rankingsarjojen ja osakilpailujen järjestämiselle
* Tuki ohjelman automaattiseen testaamiseen vanhojen, oikein haettujen kaavioiden avulla (11 testikaaviota tässä versiossa)
* Korjauksia automaattisen testaamisen löytämiin bugeihin hakualgoritmissa 
* Tässä versiossa pystytään hakemaan kolmannen kierroksen pelejä ensimmäisen kierroksen ollessa kesken

## Versio 1.0.0.2		(julkaistu 25.11.2024)

* Ensimmäinen julkaistu versio
 
