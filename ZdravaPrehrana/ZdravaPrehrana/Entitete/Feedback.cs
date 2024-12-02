using System;
using ZdravaPrehrana.Entitete;
public class Feedback {
	private int ocena;
	private string komentar;
	private DateTime datum;

	public void DodajOceno(ref int ocena) {
		throw new System.NotImplementedException("Not implemented");
	}
	public void DodajKomentar(ref string komentar) {
		throw new System.NotImplementedException("Not implemented");
	}

	private UpravljalecFeedbacka upravljaFeedback;

	private Uporabnik podaFeedback;
	private Recept jeZaRecept;

}
