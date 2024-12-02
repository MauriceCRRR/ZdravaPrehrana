using System;
using System.Runtime.InteropServices;

public class ReceptiOkno {
	public void PrikaziSeznamreceptov([Optional, DefaultParameterValueAttribute()]ref list<recept> recepti) {
		throw new System.NotImplementedException("Not implemented");
	}
	public void PrikaziPodrobnostiRecepta(ref Recept recept) {
		throw new System.NotImplementedException("Not implemented");
	}
	public void PrikaziObrazecZaDodajanje() {
		throw new System.NotImplementedException("Not implemented");
	}
	public void PrikaziObrazecZaUrejanje(ref Recept recept) {
		throw new System.NotImplementedException("Not implemented");
	}

	private GlavnoOkno odpreReceptiOkno;

}
