using UnityEngine;
using System.Collections;

public class Stage {
	string name;
	int id;
	int timelimit; //in seconds
	bool unlocked;
	string description;
	bool real; //a real scene is one actually played on, as opposed to title screen or credits screen
	private void Initialize(string n, int i, int t, bool u,string d, bool r)
	{
		name = n;
		id = i;
		timelimit = t;
		unlocked = u;
		description = d;
		real = r;
	}
	public Stage(string n, int i, int t)
	{
		Initialize(n, i, t, false,"",true);
	}
	public Stage(string n, int i, int t, bool u)
	{
		Initialize(n, i, t, u,"",true);
	}
	public Stage(string n,int i, int t, bool u, string d)
	{
		Initialize(n, i, t, u, d,true);
	}
	public Stage(string n,int i, int t, bool u, string d, bool r)
	{
		Initialize(n, i, t, u, d, r);
	}
	public bool isUnlocked()
	{
		return unlocked;
	}
	
	public void unlock()
	{
		unlocked = true;
	}
	
	public int level()
	{
		return id;
	}
	
	public int levelTime()
	{
		return timelimit;
	}
	
	public string levelName()
	{
		return name;
	}

	public string levelDesc()
	{
		return description;
	}

	public bool RealStage()
	{
		return real;
	}
}
