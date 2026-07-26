using System.Collections.Generic;
using System.Linq;

// ============================================================
// 1.  CORE TYPES
// ============================================================

public enum Phase
{
	     Solid, Liquid, Gas
}

/// <summary>
/// Represents any kind of matter (pure compound, mixture, etc.)
/// </summary>
public abstract class Matter
{
	     public float Mass
	     {
		          get; set;
	     }               // kg
	     public float Density
	     {
		          get; set;
	     }            // kg/m³
	     public ChemicalEnergy ChemicalEnergy
	     {
		          get; set;
	     }
	     public MechanicalEnergy MechanicalEnergy
	     {
		          get; set;
	     }
	     public ThermalEnergy ThermalEnergy
	     {
		          get; set;
	     }

	     /// <summary>
	     /// Returns the specific heat (J/kg·K) at the current temperature.
	     /// Must be implemented by derived classes.
	     /// </summary>
	     public abstract float GetSpecificHeat(float temperatureKelvin);

	     /// <summary>
	     /// Returns the density (kg/m³) at the current temperature.
	     /// </summary>
	     public abstract float GetDensity(float temperatureKelvin);
}

/// <summary>
/// A pure chemical compound with a fixed molecular formula.
/// </summary>
public class Compound : Matter
{
	     public string Name
	     {
		          get;
	     }
	     public string Formula
	     {
		          get;
	     }
	     public IReadOnlyDictionary<Element, int> Composition
	     {
		          get;
	     }
	     public float MolecularWeight
	     {
		          get;
	     }

	     // Phase change properties (still stored per compound)
	     public float MeltingPoint
	     {
		          get;
	     }
	     public float BoilingPoint
	     {
		          get;
	     }
	     public float LatentHeatFusion
	     {
		          get;
	     }
	     public float LatentHeatVaporization
	     {
		          get;
	     }
	     public float ThermalConductivity
	     {
		          get;
	     }
	     public float ElectricalConductivity
	     {
		          get;
	     }

	     // No tables stored here – just a material key for library lookups
	     private readonly string _materialKey;

	     public Compound(
	         string name,
	         string formula,
	         Dictionary<Element, int> composition,
	         float molecularWeight,
	         float meltingPoint,
	         float boilingPoint,
	         float latentHeatFusion,
	         float latentHeatVaporization,
	         float thermalConductivity,
	         float electricalConductivity,
	         float defaultDensity = 1000f)
	     {
		          Name = name;
		          Formula = formula;
		          Composition = composition;
		          MolecularWeight = molecularWeight;
		          MeltingPoint = meltingPoint;
		          BoilingPoint = boilingPoint;
		          LatentHeatFusion = latentHeatFusion;
		          LatentHeatVaporization = latentHeatVaporization;
		          ThermalConductivity = thermalConductivity;
		          ElectricalConductivity = electricalConductivity;
		          Density = defaultDensity;
		          _materialKey = name; // or formula – whichever is used in the library

		          ThermalEnergy = new ThermalEnergy(0, name, latentHeatFusion, latentHeatVaporization, meltingPoint, boilingPoint, thermalConductivity, SimulationConstants.Kelvin, 4186);
	     }

	     public override float GetSpecificHeat(float temperatureKelvin)
	     {
		          // Delegate to the library – tables are managed centrally
		          return MatterLibrary.GetSpecificHeat(_materialKey, temperatureKelvin);
	     }

	     public override float GetDensity(float temperatureKelvin)
	     {
		          // You can add a similar method in MatterLibrary for density if needed
		          // For now, return the constant default density
		          return Density;
	     }
}
/// <summary>
/// A mixture of one or more compounds (e.g., air, granite, seawater).
/// </summary>
public class Mixture : Matter
{
	     public List<(Compound compound, float massFraction)> Components
	     {
		          get;
	     }

	     public Mixture(List<(Compound, float)> components)
	     {
		          Components = components;
		          // Compute mass-weighted density, etc.
		          //    Density = components.Sum(c => c.compound.Density * c.massFraction);
		          // ThermalEnergy will be created when needed.
	     }

	     public override float GetSpecificHeat(float temperatureKelvin)
	     {
		          return Components.Sum(c => c.massFraction * c.compound.GetSpecificHeat(temperatureKelvin));
	     }

	     public override float GetDensity(float temperatureKelvin)
	     {
		          return Components.Sum(c => c.massFraction * c.compound.GetDensity(temperatureKelvin));
	     }
}

// ============================================================
// 2.  ELEMENT DEFINITION
// ============================================================

public class Element
{
	     public string Name
	     {
		          get;
	     }
	     public string Symbol
	     {
		          get;
	     }
	     public float AtomicNumber
	     {
		          get;
	     }
	     public float AtomicMass
	     {
		          get;
	     }  // amu

	     public Element(string name, string symbol, float atomicNumber, float atomicMass)
	     {
		          Name = name;
		          Symbol = symbol;
		          AtomicNumber = atomicNumber;
		          AtomicMass = atomicMass;
	     }

	     public override string ToString() => Symbol;
}

