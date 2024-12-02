using System;
using System.Collections.Generic;
using ZdravaPrehrana.Entitete;

public class Jedilnik {
	public string naziv;
	public DateTime datumKreiranja;
	public List<Obrok> obroki;

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

	public Recept[] vsebujeRecept;
	public UpravljalecJedilnika upravlja;

	public Uporabnik ustvariJedilnik;
	public Uporabnik[] deliZ;

}
