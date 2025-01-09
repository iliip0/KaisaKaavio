# KaisaKaavio ohjelman versiohistoria

Tämä dokumentti sisältää ytimekkään kuvauksen mitä muutoksia ohjelman versiot sisältävät verrattuna aiempiin versioihin,

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
 
