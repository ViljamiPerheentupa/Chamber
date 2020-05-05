// Place this file into a directory called Editor inside of your Assets directory.

// Unity 5.3+:

// Models imported after you do that will have:
// - Normals set to Import.
// - Set the bool "useMikk" to true:
//   Tangents set to Calculate Tangent Space (MikkTSpace).
// - Set the bool "useMikk" to false:
//   Tangents set to Split Tangents (UnityTSpace).

// Unity 5.2-:

// Models imported after you do that will have:
// - Smoothing Angle seto to 180.
// - Normals set to Import.
// - Tangents set to Calculate (UnityTSpace).
// - Tangents set to Split Tangents.

// All Unity versions:

// Any models that are already imported can have this applied by selecting them in
// the Project panel, right clicking and selecting "Reimport" from the pop-op menu.

// MikkTSpace:
// These tools generate MikkTSpace normal maps:
// - Modo's MikkTSpace output
// - https://www.thefoundry.co.uk/products/modo/
// - Default xNormal Plugin
// - http://www.xnormal.net
// - Knald
// - https://www.knaldtech.com
// - MightyBake
// - http://www.mightybake.com
// - Blender
// - http://www.blender.org

// UnityTSpace:
// These tools generate UnityTSpace normal maps:
// - Modo's Unity output
// - http://community.thefoundry.co.uk/discussion/topic.aspx?f=83&t=77919
// - UnityTSpace xNormal Plugin
// - http://www.farfarer.com/blog/2012/06/12/unity3d-tangent-basis-plugin-xnormal/
// - Handplane3D's Unity output
// - http://www.handplane3d.com

using UnityEditor;
using UnityEngine;
using System.Collections;

public class SmoothingAngleFix : AssetPostprocessor {

	private bool useMikk = true;

	void OnPreprocessModel () {
		ModelImporter modelImporter = assetImporter as ModelImporter;

#if UNITY_5_3
		// Set Normals to Import.
		modelImporter.importNormals = ModelImporterNormals.Import;

		// Set Tangents to Calculate.
		if (useMikk) {
			modelImporter.importTangents = ModelImporterTangents.CalculateMikk;
		} else {
			modelImporter.importTangents = ModelImporterTangents.CalculateLegacyWithSplitTangents;
		}
#else
		// Set Smoothing Angle to 180.
		modelImporter.normalImportMode = ModelImporterTangentSpaceMode.Calculate;
		modelImporter.normalSmoothingAngle = 180.0f;

		// Set Normals to Import.
		modelImporter.normalImportMode = ModelImporterTangentSpaceMode.Import;

		// Set Tangents to Calculate.
		modelImporter.tangentImportMode = ModelImporterTangentSpaceMode.Calculate;

		// Set Split Tangents to True.
		modelImporter.splitTangentsAcrossSeams = true;
#endif
	}
}