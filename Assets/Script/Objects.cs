using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Public static class holding hidden objects. Used for instantiating new objects
public static class Objects {
	// list of objects
	public static Ship CarrierHull;
	public static Ship CruiserHull;
	public static Gun GunS;
	public static Gun GunM;
	public static GameObject TrailOne;
	public static GameObject TrailTwo;
	public static Bullet BulletS;
	public static Bullet BulletM;
	public static SpawnPoint Spawn;
	public static GameObject Border;

	// list of ship types
	public static Carrier CarrierVars;
	public static Cruiser CruiserVars;
}