using System;

public class Uporabnik {
	private string uporabniskoIme;
	private string geslo;
	private enum vloga;

	public bool PreveriPrijavo(ref string uporabniskoIme, ref String geslo) {
		throw new System.NotImplementedException("Not implemented");
	}
	public UporabnikProfil PridobiProfil() {
		throw new System.NotImplementedException("Not implemented");
	}

	private UporabnikProfil imaProfil;
	private Jedilnik[] ustvariJedilnik;
	private PrehranskiCilji ustvariCilj;
	private VnosHranil[] vnese;
	private Nasvet[] pridobiNasvet;
	private Feedback[] podaFeedback;
	private NakupovalniSeznam[] ustvariSeznam;
	private UpravljalecDeljenja deliVsebinoZ;
	private Jedilnik deliZ;
	private UpravljalecNasvetov gemeriraZa;
	private UpravljalecFeedbacka prejmeOd;

}
