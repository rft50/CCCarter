using System;
using System.Collections.Generic;

namespace Carter.External;

public interface IDuoApi
{
	Deck DuoArtifactVanillaDeck { get; }
	void RegisterDuoArtifact(Type type, IEnumerable<Deck> combo);
}