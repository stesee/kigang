using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace ClassLibraryDatabase
{
    //  Class bases on this Draft:
    //    https://svnmr.technikum-wien.at/svn/BMT_MGR3_AGT/trunk/KiGang/Umsetzung/Anforderungen/Entity-Diagramm.jpeg
    public class Therapie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Kommentar { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
        public virtual ICollection<Therapeut> Therapeuts { get; set; }
        public virtual ICollection<Trainingseinheit> Trainingseinheiten { get; set; }
        //buggy with ef 4.1 using string instead
        // found bug in entityframework :( http://blog.brettski.com/2010/12/25/ef-code-first-datetime2-to-datetime-out-of-range/
        //public DateTime Beginn { get; set; }
        public string Beginn { get; set; }
        //buggy with ef 4.1 using string instead
        //public DateTime Ende { get; set; }
        public string Ende { get; set; }
        public string Diagnose { get; set; }
        //Todo: Discuss neccacarity of bla AnzahlSessions
        //public int AnzahlSessions { get; set; }
    }
    public class Therapeut
    {
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Kommentar { get; set; }
        public string Strasse { get; set; }
        public int Plz { get; set; }
        public string Ort { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public int SvNr { get; set; }
        public string Type { get; set; }
    }
    public class Trainingseinheit
    {
        public int Id { get; set; }
        //buggy with ef 4.1 using string instead
        //public DateTime Timestamp { get; set; }
        public string Timestamp { get; set; }

        // 0 - 1 
        public virtual Uebungsplan Uebungsplan { get; set; }
        // 0 - *
        public virtual ICollection<Bewegungsaufzeichnung> Bewegungsaufzeichnung { get; set; }
    }
    public class Uebungsgeraete
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
    }
    public class Uebungsplan
    {
        public int Id { get; set; }
        public string ZuUebenBis { get; set; }
        public virtual ICollection<Ueabungsaufgabe> Ueabungsaufgabe { get; set; }

    }
    public class Ueabungsaufgabe
    {
        public int Id { get; set; }
        public virtual Uebung Uebung { get; set; }
        public int AnzWiederhlg { get; set; }
        public virtual ICollection<Uebungsgeraete> Uebungsgeraete { get; set; }

    }
    public class Uebung
    {
        public int Id { get; set; }
        public string UebungsName { get; set; }
        public virtual Bewegungsaufzeichnung Bewegungsaufzeichnung { get; set; }
    }
    public class Patient
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string Kommentar { get; set; }
    }
    public class Bewegungsaufzeichnung
    {
        public int Id { get; set; }
        //buggy with ef 4.1 using string instead
        public string Zeit { get; set; }
        //Verweis auf die Übungsaufgabe, welche als Referenz genommen wird.
        //Uebungsaufgabe=NULL wenn diese Bewegungsaufzeichnung eine Referenz ist.
        public Ueabungsaufgabe Ueabungsaufgabe { get; set; }
        //Key: Framenummer, welche den SkelettPositionsdaten zugeordnet ist.
        //SkeletonData: SkelettPositionsdaten: Spezielle Persisiterungsklasse muss noch definiert werden.
        public virtual ICollection<SpecialSkeletonData> skeletonData { get; set; }
    }
    public class SpecialSkeletonData
    {
        //primärschlüssel(Id) für DB, wird automatisch befüllt
        public int Id { get; set; }
        public int FrameNr { get; set; }
        public virtual ICollection<SkeletonJointData> kigangSkelleton { get; set; }
        public string Timestamp { get; set; }
    }
    public class SkeletonJointData
    {
        //primärschlüssel(Id) für DB, wird automatisch befüllt
        public int Id { get; set; }
        public int jointId { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public double w { get; set; }
    }
}
