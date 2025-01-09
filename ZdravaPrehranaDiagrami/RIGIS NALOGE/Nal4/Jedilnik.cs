using System;

public class Jedilnik {
	private string naziv;
	private date datumKreiranja;
	private List<Obrok> obroki;

	public bool KreirajJedilnik() {
		throw new System.NotImplementedException("Not implemented");
	}
	public void DodajObrok(ref Obrok obrok) {
		throw new System.NotImplementedException("Not implemented");
	}
	public void OdstraniObrok(ref Obrok obrok) {
		throw new System.NotImplementedException("Not implemented");
	}
	public void DeliJedilnik(ref Uporabnik uporabnik) {
		throw new System.NotImplementedException("Not implemented");
	}

	private Recept[] vsebujeRecept;
	private UpravljalecJedilnika upravlja;

	private Uporabnik ustvariJedilnik;
	private Uporabnik[] deliZ;

}
