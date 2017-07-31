using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum POWER_TRANSFER_STATE
{

    CHARGING_STATION,
    CHARGING_BATTERIES,
    DRAINING_STATION,
    DRAINING_BATTERIES,
    EQUALIBRIUM,
    OVERCHARGING

}


public class PowerManager : MonoBehaviour {

    public POWER_TRANSFER_STATE pts = POWER_TRANSFER_STATE.EQUALIBRIUM;

    public GameObject[] solar_panels;
    public GameObject[] batteries;

    public GameObject battery_status_icon;

    public float charge_rate = 0.1f;
    public float drain_rate = 0.1f;

    public float charge_scale;
    public float drain_scale;

    public float current_station_charge;
    public float current_batteries_charge;

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


        




        //get number of active devices
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


        charge_scale = (float)sp_working_count / (float)sp_total_count;
        drain_scale = 0.005f;

        float total_power_gain = charge_scale * charge_rate;
        float total_power_drain = drain_scale * drain_rate;

        float power_diff = total_power_gain - total_power_drain;









        //ratios
        batteries_charge_capacity = (float) b_working_count / (float) b_total_count;
        battery_count = b_working_count;

        current_station_charge += power_diff;

        print(power_diff);
        print(total_power_gain + "\t" + total_power_drain);










        if (power_diff > 0.0f) //GAINING POWER
        {
            if (current_station_charge >= 1.0f) //CHARGING BATTERIES
            {

                if(current_batteries_charge <= batteries_charge_capacity)
                {
                    pts = POWER_TRANSFER_STATE.CHARGING_BATTERIES;
                    float overflow = current_station_charge - 1.0f;
                    current_batteries_charge += overflow;
                }
                else
                {
                    pts = POWER_TRANSFER_STATE.OVERCHARGING;
                }

                current_station_charge = 1.0f;

            }
            else
            {
                pts = POWER_TRANSFER_STATE.CHARGING_STATION;
            }


        }
        else if (power_diff < 0.0f) //LOSING POWER
        {

            pts = POWER_TRANSFER_STATE.DRAINING_STATION;


            if (current_station_charge < 1.0f && current_batteries_charge > 0.0f)
            {
                pts = POWER_TRANSFER_STATE.DRAINING_BATTERIES;// & POWER_TRANSFER_STATE.CHARGING_STATION;

                current_batteries_charge += power_diff;
                current_station_charge -= power_diff;

            }

        }
        else //POWER AT EQUALIBRIUM
        {
            pts = POWER_TRANSFER_STATE.EQUALIBRIUM;
        }











        //Battery visuals
        int max_cells = battery_count * 7;
        int num_cells = (int) (max_cells * current_batteries_charge);
        int cells_per_battery = 0;

        if (battery_count > 0 && current_batteries_charge > 0)
        {
        cells_per_battery = num_cells / battery_count + 1;
        }

        foreach (GameObject g in batteries)
        {
            if (!g.GetComponent<StationBattery>().isBroken)
            {
                g.GetComponent<StationBattery>().setChargeLevel(cells_per_battery);
            }

        }




        //lights visuals
        if (enable_station_lights)
        {
            current_color = Color.Lerp(no_power_light_color, full_power_light_color, current_station_charge);
        }
        else
        {
            current_color = no_power_light_color;
        }

        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, current_color, 0.1f);



        //print power stuff
        //print("current station charge: " + current_station_charge + "\tpossible battery fill: " + batteries_charge_capacity + "\tcurrent battery charge: " + current_batteries_charge + "\tcells: " + num_cells + " | " + cells_per_battery);

        bool batt_drain_state = false;

        if (pts == POWER_TRANSFER_STATE.CHARGING_BATTERIES)
        {
            print("CHARGING_BATTERIES");
            battery_status_icon.GetComponent<Animator>().SetBool("drain", false);
            batt_drain_state = true;
        }
        else if (pts == POWER_TRANSFER_STATE.CHARGING_STATION)
        {
            print("CHARGING_STATION");
        }
        else if (pts == POWER_TRANSFER_STATE.DRAINING_BATTERIES)
        {
            print("DRAINING_BATTERIES");
            battery_status_icon.GetComponent<Animator>().SetBool("drain", true);
            batt_drain_state = true;
        }
        else if (pts == POWER_TRANSFER_STATE.DRAINING_STATION)
        {
            print("DRAINING_STATION");
        }
        else if (pts == POWER_TRANSFER_STATE.EQUALIBRIUM)
        {
            print("EQUALIBRIUM");
        }else if (pts == POWER_TRANSFER_STATE.OVERCHARGING)
        {
            print("OVERCHARGING");
        }
        else
        {
            print("NONE");
        }

        battery_status_icon.SetActive(batt_drain_state);

    }


    float map(float val, float from_min, float from_max, float to_min, float to_max)
    {
        return (val - from_min) / (from_max - from_min) * (to_max - to_min) + to_min;
    }

}
