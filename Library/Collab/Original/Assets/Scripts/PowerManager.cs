using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour {

    public GameObject[] solar_panels;
    public GameObject[] batteries;

    public float charge_rate;
    public float power_burn;

    private float current_station_charge;

    private float current_batteries_charge;
    private float batteries_charge_capacity;
    private int battery_count;

    private float current_station_power_production;

    private int sp_total_count;
    private int sp_working_count;
    private int b_total_count;
    private int b_working_count;


    public bool enable_station_lights;


    public Color current_color;

    public Color full_power_light_color;
    public Color no_power_light_color;

    public Color danger_color;

    void Start () {

        solar_panels = GameObject.FindGameObjectsWithTag("solar");
        batteries = GameObject.FindGameObjectsWithTag("battery");

        current_station_charge = 0;

    }

	void Update () {

        sp_total_count = solar_panels.Length;
        b_total_count = batteries.Length;

        sp_working_count = 0;
        b_working_count = 0;


        foreach (GameObject g in solar_panels)
        {
            if (!g.GetComponent<SolarArray>().isBroken)
                sp_working_count++;
        }

        foreach (GameObject g in batteries)
        {
            if (!g.GetComponent<StationBattery>().isBroken)
                b_working_count++;
        }


        current_station_power_production = (float) sp_working_count / (float) sp_total_count;
        batteries_charge_capacity = (float) b_working_count / (float) b_total_count;

        battery_count = b_working_count;

        //print("current station production: " + current_station_power_production + "\tcurrent station charge: " + current_station_charge);

        if(current_station_charge < 1)
        {
            current_station_charge += charge_rate * current_station_power_production;
        }else if(current_batteries_charge < batteries_charge_capacity)
        {
            current_batteries_charge += charge_rate * current_station_power_production;
        }

        int max_cells = battery_count * 8;
        int num_cells = (int) (max_cells * current_batteries_charge);
        int cells_per_battery = 0;

        if (battery_count > 0)
        {
        cells_per_battery = num_cells / battery_count;
        }

        foreach (GameObject g in batteries)
        {
            if (!g.GetComponent<StationBattery>().isBroken)
            {
                g.GetComponent<StationBattery>().setChargeLevel(cells_per_battery);
            }

        }


        //check for extra power, put into batteries
        if (current_station_charge < 1 && current_batteries_charge > 0)
        {
            //transfer power from battery to station
            current_station_charge += charge_rate;
            current_batteries_charge -= charge_rate;
        }



        //all power drawing stuff
        if (enable_station_lights)
        {

            if (current_station_charge > 0)
                current_station_charge -= power_burn;

            current_color = Color.Lerp(no_power_light_color, full_power_light_color, current_station_charge);

        }
        else
        {
            current_color = no_power_light_color;
        }

        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, current_color, 0.1f);


        //print power stuff
        print("current station charge: " + current_station_charge + "\tpossible battery fill: " + batteries_charge_capacity + "\tcurrent battery charge: " + current_batteries_charge + "\tcells: " + num_cells + " | " + cells_per_battery);


    }


    float map(float val, float from_min, float from_max, float to_min, float to_max)
    {
        return (val - from_min) / (from_max - from_min) * (to_max - to_min) + to_min;
    }

}
