using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using Silk.NET.SDL;
using Color = Jypeli.Color;
///@author vesaloko
/// @version 14.11.2022
/// <summary>
/// ohjelmointi 1 -kurssin harkkatyönä toteutettu tanssipeli
/// </summary>

namespace tanssipeli;

public class tanssipeli : PhysicsGame
{
    private PhysicsObject tanssija;
    private PhysicsObject nuoli;

    private Image taustakuva = LoadImage("pelli.tausta.png");
    private Image oikea = LoadImage("kasi ylos_1");
    private Image vasen = LoadImage("kasiaalto_2");
    private Image[] ylos = LoadImages("hyppy_1", "hyppy_2");
    private Image[] alas = LoadImages("spagaati_1", "spagaati_2", "spagaati_3");
    private Image nuolenKuva = LoadImage("nuoli");

    private IntMeter pistelaskuri;
    private EasyHighScore topLista = new EasyHighScore();

    private List<PhysicsObject> liikutettavat = new List<PhysicsObject>();


    public override void Begin()
    {
        Kaynnistys();
        LuoKentta();
        LuoNuolet();

        Timer.CreateAndStart(2, LiikutaOlioita);
 
        Jypeli.Surface alareuna = Jypeli.Surface.CreateBottom(Level, 30, 30, 2, 0);
        Add(alareuna);

        Keyboard.Listen(Key.Right, ButtonState.Pressed, LiikutaPelaajaaOikealle, null);
        Keyboard.Listen(Key.Left, ButtonState.Pressed, LiikutaPelaajaaVasemmalle, null);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, LiikutaPelaajaaYlos, null);
        Keyboard.Listen(Key.Down, ButtonState.Pressed, LiikutaPelaajaaAlas, null);
    }


    /// <summary>
    /// luodaan peliin nuolet
    /// </summary>
    private void LuoNuolet()
    {
        int nuoltenmäärä = 100;
        for (int i = 0; i < nuoltenmäärä; i++)
        {
            PhysicsObject nuoli = new PhysicsObject(50, 50);
            nuoli.Image = nuolenKuva;
            nuoli.X = 0;
            nuoli.Y = -200;
            nuoli.Angle = RandomGen.SelectOne(Angle.FromDegrees(90), Angle.FromDegrees(180), Angle.FromDegrees(270), Angle.FromDegrees(0));

            liikutettavat.Add(nuoli);
        }

    }


    /// <summary>
    /// pelaaja liikkuu alas, piste oikeasta nuolesta
    /// </summary>
    /// <returns></returns>
    private void LiikutaPelaajaaAlas()
    {
        tanssija.Image = LoadImage("spagaati_3");
        tanssija.Image.ReplaceColor(Color.White, Color.Transparent);

        Angle verrattavakulma = Angle.FromDegrees(270);
        Angle piste = liikutettavat[0].Angle;
        if (piste == verrattavakulma)pistelaskuri.Value += 1;
        if (piste != verrattavakulma) Haviaminen();
    }


    /// <summary>
    /// miten hahmo liikkuu kun painetaan oikealle, piste oikeasta nuolesta
    /// </summary>
    private void LiikutaPelaajaaOikealle()
    {
        tanssija.Image = LoadImage("kasiaalto_2");
        tanssija.Image.ReplaceColor(Color.White, Color.Transparent);

        Angle verrattavakulma = Angle.FromDegrees(0);
        Angle piste = liikutettavat[0].Angle;
        if (piste == verrattavakulma) pistelaskuri.Value += 1;
        if (piste != verrattavakulma) Haviaminen();
    }


    /// <summary>
    /// miten hahmo liikkuu vasemmalle painettaessa, piste oikeasta nuolesta
    /// </summary>
    private void LiikutaPelaajaaVasemmalle()
    {
        tanssija.Image = LoadImage("kasi ylos_1");
        tanssija.Image.ReplaceColor(Color.White, Color.Transparent);

        Angle verrattavakulma = Angle.FromDegrees(180);
        Angle piste = liikutettavat[0].Angle;
        if (piste == verrattavakulma) pistelaskuri.Value += 1;
        if (piste != verrattavakulma) Haviaminen();
    }


    /// <summary>
    /// tanssija liikkuu ylös painattaessa, piste oikeasta nuolesta
    /// </summary>
    private void LiikutaPelaajaaYlos()
    {
        tanssija.Image = LoadImage("hyppy_2");
        tanssija.Image.ReplaceColor(Color.White, Color.Transparent);

        Angle verrattavakulma = Angle.FromDegrees(90);
        Angle piste = liikutettavat[0].Angle;
        if (piste == verrattavakulma) pistelaskuri.Value += 1;
        if (piste != verrattavakulma) Haviaminen();
    }


    /// <summary>
    /// luodaan peliin kenttä, ja siinä esiintyvät asiat
    /// </summary>
    private void LuoKentta()
    {
        Level.Background.Image = taustakuva;

        tanssija = PhysicsObject.CreateStaticObject(600, 700);
        tanssija.X = -320;
        tanssija.Y = -100;
        tanssija.Image = LoadImage("seisominen");
        tanssija.Image.ReplaceColor(Color.White, Color.Transparent);
        Add(tanssija);

        LuoPistelaskuri();
    }


    /// <summary>
    /// luodaan peliin pistelaskuri, joka laskee pisteet 50 asti
    /// </summary>
    private void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);

        Label pistenaytto = new Label(150, 50);
        pistenaytto.X = Screen.Right - 500;
        pistenaytto.Y = Screen.Top - 50;
        pistenaytto.TextColor = Color.Black;
        pistenaytto.Color = Color.LightBlue;
        pistenaytto.Title = "Pisteet: ";

        pistenaytto.BindTo(pistelaskuri);
        Add(pistenaytto);

        IntMeter keratytEsineet = new IntMeter(0);
        pistelaskuri.MaxValue = 50;
        pistelaskuri.UpperLimit += KaikkiKeratty;
    }



    /// <summary>
    /// voittoteksti 50 pisteen jälkeen
    /// </summary>
    private void KaikkiKeratty()
    {
        if (pistelaskuri.Value == 50) MessageDisplay.Add("Voitit pelin!");
        MessageDisplay.X = Screen.Right - 500;
        MessageDisplay.Y = Screen.Top - 100;
        MessageDisplay.TextColor = Color.Black;
        MessageDisplay.Color = Color.LightBlue;

        topLista.EnterAndShow(pistelaskuri.Value);
        topLista.HighScoreWindow.Closed += AloitaPeli;
    }


    /// <summary>
    /// aloitetaan uusi peli
    /// </summary>
    /// <param name="sender"></param>
    private void AloitaPeli(Jypeli.Window sender)
    {
        pistelaskuri.Reset();
        Kaynnistys();
    }


    /// <summary>
    /// taulukkoon sijoitetut nuolet liikkeelle ja pois taulukosta
    /// </summary>
    private void LiikutaOlioita()
    {
        Add(liikutettavat[1]);
        liikutettavat[1].Hit(new Vector(800, 10));
        liikutettavat.RemoveAt(0);
        liikutettavat.RemoveAt(1);
    }


    /// <summary>
    /// pelin käynnistys ja alkuvalikko
    /// </summary>
    private void Kaynnistys()
    {
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Pelin alkuvalikko", "Aloita peli", "Parhaat pisteet", "Lopeta");
        Add(alkuvalikko);

        alkuvalikko.AddItemHandler(0, AloitaPelit);
        alkuvalikko.AddItemHandler(1, ParhaatPisteet);
        alkuvalikko.AddItemHandler(2, Exit);

        alkuvalikko.DefaultCancel = 3;

        alkuvalikko.Color = Color.LightBlue;
        alkuvalikko.SetButtonColor(Color.White);
        alkuvalikko.SetButtonTextColor(Color.Black);
     
        List<Label> valikonKohdat;

        Label otsikko = new Label("Tanssipelin Alkuvalikko"); 
        otsikko.Y = 100; 
        otsikko.Font = new Font(50, true); 
        
        valikonKohdat = new List<Label>(); 

        Label kohta1 = new Label("Aloita uusi peli");  
        kohta1.Position = new Vector(0, 40); 

        Label kohta2 = new Label("Parhaat pisteet");
        kohta2.Position = new Vector(0, 0);

        Label kohta3 = new Label("Lopeta peli");
        kohta3.Position = new Vector(0, -40);

        foreach (Label valikonKohta in valikonKohdat)
        {
            Add(valikonKohta);
        }

        Mouse.ListenOn(kohta1, MouseButton.Left, ButtonState.Pressed, AloitaPelit, null);
        Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, ParhaatPisteet, null);
        Mouse.ListenOn(kohta3, MouseButton.Left, ButtonState.Pressed, Exit, null);

        Mouse.ListenOn(kohta1, HoverState.Enter, MouseButton.None, ButtonState.Irrelevant, ValikossaLiikkuminen, null, kohta1, true);
        Mouse.ListenOn(kohta1, HoverState.Exit, MouseButton.None, ButtonState.Irrelevant, ValikossaLiikkuminen, null, kohta1, false);
    }


    /// <summary>
    /// liikutaan valikossa
    /// </summary>
    /// <param name="kohta"></param>
    /// <param name="paalla"></param>
    private void ValikossaLiikkuminen(Label kohta, bool paalla)
    {
        if (paalla)
        {
            kohta.TextColor = Color.Gray;
        }
        else
        {
            kohta.TextColor = Color.Black;
        }
    }


    /// <summary>
    /// valikon painikkeet
    /// </summary>
    private void AloitaPelit()
    {
  
    }


    /// <summary>
    /// valikon painikkeet
    /// </summary>
    private void ParhaatPisteet()
    {
        KaikkiKeratty();
    }


    /// <summary>
    /// väärästä pisteestä pelaaja häviää ja aukeaa top-lista
    /// </summary>
    private void Haviaminen()
    {
        Explosion rajahdys = new Explosion(50);
        rajahdys.Position = tanssija.Position;
        Add(rajahdys);


        MessageDisplay.Add("Hävisit pelin!");
        MessageDisplay.X = Screen.Right - 500;
        MessageDisplay.Y = Screen.Top - 100;
        MessageDisplay.TextColor = Color.Black;
        MessageDisplay.Color = Color.LightBlue;

        topLista.EnterAndShow(pistelaskuri.Value);
        topLista.HighScoreWindow.Closed += AloitaPeli;
    }

}