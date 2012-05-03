using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Diese Klassen sind zuständig für die zentrale Steuerung der supervidierten Anwendung.
 * (Business-Logik)
 * Verwaltung und Zusammenführung der Schnittstellen GUI, Kinect und Datenbank
 *
 * Diskussionspunkt: Soll eigene Klasse für Koordinaten definiert werden?
 * Vorteil: Unabhängigkeit von Microsoft SDK
 * */

namespace SVT
{
    /**
     * Controller für Usecase beim Therapeuten
     * */
    
    public class SVTControllerTherapeut
    {
        
        ///Methoden für Usecase beim Therapeuten
        
        /**
         * Startet eine Bewegungsaufzeichnung, Koordinaten werden in einem Objekt zwischengespeichert.
         * Return: Name der Bewegungsaufzeichnung
         */
        public string startrecording()
        {
            //SkeletonFrameListener einhängen und Koordinaten in ein Objekt speichern
            return "";
        }

        /**
         * Stoppt eine aktive Operation (Aufzeichnung oder Wiedergabe)
         * */
        public void stop(string name) {

        }

        /**
         * Startet die Wiedergabe der mit name bezeichneten Bewegungsaufzeichnung
         * Es können sowohl Objekte im Speicher oder bereits persistierte Aufzeichnungen aus der Datenbank abgespielt werden.
         * 
         * TODO: In jedem Frame wird das Skelettmodell gezeichnet und an den EventHandler übergeben.
         * */
        public void startplaying(string name, EventHandler videorenderer)
        {
            
        }

        /**
         * Persistiert Bewegungsaufzeichnungsobjekt in die Datenbank. 
         * */
        public void save(string name)
        {
        }

        /**
         * Löscht Bewegungsaufzeichnung mit dem Namen name
         * */
        public void delete(string name)
        { }
    }

    /**
     * Controller für Usecase beim Patienten
     * */
    public class SVTControllerPatient
    {
        /**
         * Startet Übungsausführung: Wiedergabe Live-Video, Skelettmodell aus Aufzeichnung und Darstellung bei Abweichung
         * */
        public void startexercise(string name, EventHandler videorenderer)
        { }

        /**
         * Übungsausführung pausieren
         * */
        public void pause(string name)
        { }

        /**
         * Übungsausführung stoppen
         * */
        public void stop(string name)
        { }

    }
}
