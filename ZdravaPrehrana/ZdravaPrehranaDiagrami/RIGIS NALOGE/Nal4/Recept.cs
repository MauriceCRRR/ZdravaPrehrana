using System;

public class Recept {
	private string naziv;
	private List<Sestavina> sestavine;
	private string postopek;
	private int kalorije;
	private int cas_priprave;

	public void DodajSestavino(ref Sestavina sestavina) {
		throw new System.NotImplementedException("Not implemented");
	}
	public bool UrediRecept() {
		throw new System.NotImplementedException("Not implemented");
	}
	public double IzracunHranilneVrednosti() {
		throw new System.NotImplementedException("Not implemented");
	}

	private Feedback jeZaRecept;
	private UpravljalecReceptov upravlja;

	private Jedilnik vsebujeRecept;

}
