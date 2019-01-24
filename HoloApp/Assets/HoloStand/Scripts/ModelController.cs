using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using UnityEngine;

namespace HoloStand
{
    public class ModelController : InteractionReceiver
    {
        public Material WaterLeft;
        public Material WaterRight;
        public Material Default;
        public Material Manometer;
        public Material WaterBa;
        public Material NoWater;
        private List<bool> triggers;

        // Use this for initialization
        void Start()
        {
            triggers = new List<bool>();
            for (int i = 0; i < interactables.Count; i++)
            {
                triggers.Add(false);
            }
        }

      protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            switch (obj.name)
            {
                case "ButtonStand":
                    if (triggers[GetControlNumber(obj)])
                    {
                        transform.Find("Tube (1)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (2)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (3)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (4)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (5)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (6)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (7)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (8)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (9)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (10)").GetComponent<MeshRenderer>().material = Default;
                        
                        transform.Find("Tube (11)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (12)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (14)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (17)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (18)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (19)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = Default;

                        transform.Find("Tube (21)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (22)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (23)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (24)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (26)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (27)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (28)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (29)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (29)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (30)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (31)").GetComponent<MeshRenderer>().material = Default;
                        
                        transform.Find("Cube_C").GetComponent<MeshRenderer>().material = NoWater;
                        transform.Find("Val").gameObject.SetActive(true);
                        transform.Find("Object028 (1)").gameObject.SetActive(false);
                        transform.Find("Val (1)").gameObject.SetActive(true);
                        transform.Find("Object028 (2)").gameObject.SetActive(false);
                        transform.Find("Val (2)").gameObject.SetActive(true);
                        transform.Find("Object028 (3)").gameObject.SetActive(false);
                        transform.Find("Val (3)").gameObject.SetActive(true);
                        transform.Find("Object028 (4)").gameObject.SetActive(false);
                        transform.Find("Val (4)").gameObject.SetActive(true);
                        transform.Find("Object028 (5)").gameObject.SetActive(false);
                        transform.Find("Val (5)").gameObject.SetActive(true);
                        transform.Find("Object028 (6)").gameObject.SetActive(false);
                        transform.Find("Val (6)").gameObject.SetActive(true);
                        transform.Find("Object028 (7)").gameObject.SetActive(false);
                        transform.Find("Val (7)").gameObject.SetActive(true);
                        transform.Find("Object028 (8)").gameObject.SetActive(false);
                        transform.Find("Val (8)").gameObject.SetActive(true);
                        transform.Find("Object028 (9)").gameObject.SetActive(false);
                        triggers[GetControlNumber(obj)] = false;
                        triggers[1] = false;
                        triggers[2] = false;
                        triggers[3] = false;
                        triggers[4] = false;
                        triggers[5] = false;
                        triggers[6] = false;
                        triggers[7] = false;
                        triggers[8] = false;
                    }
                    else
                    {
                        transform.Find("Tube").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (1)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (2)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (3)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (4)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (5)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (6)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (7)").GetComponent<MeshRenderer>().material = WaterLeft;
                        transform.Find("Tube (9)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (10)").GetComponent<MeshRenderer>().material = WaterLeft;
                        transform.Find("Tube (28)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Tube (29)").GetComponent<MeshRenderer>().material = WaterRight;
                        transform.Find("Cube_C").GetComponent<MeshRenderer>().material = WaterBa;
                        
                        transform.Find("Val").gameObject.SetActive(false);
                        transform.Find("Object028 (1)").gameObject.SetActive(true);

                        Debug.Log(GetControlNumber(obj));
                        triggers[GetControlNumber(obj)] = true;
                    }

                    break;
                case "ButtonStand (1)":

                    if (triggers[GetControlNumber(obj)])
                    {
                        transform.Find("Tube (30)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Tube (31)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Val (1)").gameObject.SetActive(true);
                        transform.Find("Object028 (2)").gameObject.SetActive(false);
                        triggers[GetControlNumber(obj)] = false;
                    }
                    else
                    {
                        if (triggers[0])
                        {
                            transform.Find("Tube (30)").GetComponent<MeshRenderer>().material = WaterRight;
                            transform.Find("Tube (31)").GetComponent<MeshRenderer>().material = WaterRight;
                            transform.Find("Val (1)").gameObject.SetActive(false);
                            transform.Find("Object028 (2)").gameObject.SetActive(true);
                            triggers[GetControlNumber(obj)] = true;
                        }
                        
                    }

                    break;
                case "ButtonStand (2)": 
                    if (triggers[GetControlNumber(obj)])
                    {                     
                        if (triggers[0])
                        {
                            if (triggers[4])
                            { triggers[4] = false;
                                if (triggers[5])
                                {
                                    
                                    triggers[GetControlNumber(obj)] = false;
                                    transform.Find("Tube (8)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (12)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = WaterLeft;
                                    transform.Find("Cylinder.001_C").GetComponent<MeshRenderer>().material = NoWater;
                                    transform.Find("Cylinder.054_C").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Val (2)").gameObject.SetActive(true);
                                    transform.Find("Object028 (3)").gameObject.SetActive(false);
                                    transform.Find("Val (4)").gameObject.SetActive(true);
                                    transform.Find("Object028 (5)").gameObject.SetActive(false);
                                }
                                else
                                {
                                    triggers[GetControlNumber(obj)] = false;
                                    transform.Find("Tube (8)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (12)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Cylinder.001_C").GetComponent<MeshRenderer>().material = NoWater;
                                    transform.Find("Cylinder.054_C").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Val (2)").gameObject.SetActive(true);
                                    transform.Find("Object028 (3)").gameObject.SetActive(false);
                                    transform.Find("Val (4)").gameObject.SetActive(true);
                                    transform.Find("Object028 (5)").gameObject.SetActive(false);
                                }

                                if (triggers[8])
                                {triggers[8] = false;
                                    transform.Find("Tube (17)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Val (6)").gameObject.SetActive(true);
                                    transform.Find("Object028 (7)").gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                triggers[GetControlNumber(obj)] = false;
                                transform.Find("Tube (8)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (12)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Cylinder.001_C").GetComponent<MeshRenderer>().material = NoWater;
                                transform.Find("Val (2)").gameObject.SetActive(true);
                                transform.Find("Object028 (3)").gameObject.SetActive(false);
                                
                                
                            }

                            if (triggers[6])
                            { 
                                triggers[6] = false;
                                if (triggers[7])
                                {
                                   
                                    transform.Find("Tube (18)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (19)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = WaterLeft;
                                    transform.Find("Val (7)").gameObject.SetActive(true);
                                    transform.Find("Object028 (8)").gameObject.SetActive(false);
                                }
                                else
                                {
                                    transform.Find("Tube (18)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (19)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (21)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (26)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (27)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Val (7)").gameObject.SetActive(true);
                                    transform.Find("Object028 (8)").gameObject.SetActive(false);
                                }
                            }
                            
                        }
                        
                    }
                    else
                    {
                       
                       
                        if (triggers[0])
                        {
                            triggers[GetControlNumber(obj)] = true;
                            Debug.Log("Yes");
                            transform.Find("Tube (8)").GetComponent<MeshRenderer>().material = WaterRight;
                            transform.Find("Tube (12)").GetComponent<MeshRenderer>().material = WaterRight;
                            transform.Find("Cylinder.001_C").GetComponent<MeshRenderer>().material = WaterBa;
                            transform.Find("Val (2)").gameObject.SetActive(false);
                            transform.Find("Object028 (3)").gameObject.SetActive(true);
                        }
                       
                    }

                    break;
                case "ButtonStand (3)":
                    if ((triggers[GetControlNumber(obj)]))
                    {
                        triggers[GetControlNumber(obj)] = false;
                        if (triggers[0])
                        {
                            if (triggers[5])
                            {triggers[5] = false;
                                if (triggers[4])
                                {
                                    transform.Find("Tube (11)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (14)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = WaterLeft;
                                    transform.Find("Cylinder_C").GetComponent<MeshRenderer>().material = NoWater;
                                    transform.Find("Val (3)").gameObject.SetActive(true);
                                    transform.Find("Object028 (4)").gameObject.SetActive(false);
                                    transform.Find("Val (5)").gameObject.SetActive(true);
                                    transform.Find("Object028 (6)").gameObject.SetActive(false);
                                    transform.Find("Cylinder.055_C").GetComponent<MeshRenderer>().material = Default;
                                    
                                }
                                else
                                {
                                    transform.Find("Tube (11)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (14)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Cylinder_C").GetComponent<MeshRenderer>().material = NoWater;
                                    transform.Find("Cylinder.055_C").GetComponent<MeshRenderer>().material = Default;
                                    transform.Find("Val (3)").gameObject.SetActive(true);
                                    transform.Find("Object028 (4)").gameObject.SetActive(false);
                                    transform.Find("Val (5)").gameObject.SetActive(true);
                                    transform.Find("Object028 (6)").gameObject.SetActive(false);
                                   
                                    if (triggers[8])
                                    {triggers[8] = false;
                                        transform.Find("Tube (17)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Val (6)").gameObject.SetActive(true);
                                        transform.Find("Object028 (7)").gameObject.SetActive(false);
                                    }
                                }
                                if (triggers[7])
                                { 
                                    triggers[7] = false;
                                    if (triggers[6])
                                    {
                                   
                                        transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = WaterLeft;
                                        transform.Find("Tube (23)").GetComponent<MeshRenderer>().material = WaterLeft;
                                        transform.Find("Tube (24)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Tube (22)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Val (8)").gameObject.SetActive(true);
                                        transform.Find("Object028 (9)").gameObject.SetActive(false);
                                        transform.Find("Cylinder.055_C").GetComponent<MeshRenderer>().material = Default;
                                    }
                                    else
                                    {
                                        transform.Find("Tube (24)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Tube (22)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Tube (21)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Tube (26)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Tube (27)").GetComponent<MeshRenderer>().material = Default;
                                        transform.Find("Val (8)").gameObject.SetActive(true);
                                        transform.Find("Object028 (9)").gameObject.SetActive(false);
                                    }
                                }
                            }
                            else
                            {
                                
                                transform.Find("Tube (11)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (14)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Cylinder_C").GetComponent<MeshRenderer>().material = NoWater;
                                transform.Find("Val (3)").gameObject.SetActive(true);
                                transform.Find("Object028 (4)").gameObject.SetActive(false);
                            }
                            
                        }
                        
                    }
                    else
                    { 
                        
                        if (triggers[0])
                        {
                            triggers[GetControlNumber(obj)] = true;
                            transform.Find("Tube (11)").GetComponent<MeshRenderer>().material = WaterRight;
                            transform.Find("Tube (14)").GetComponent<MeshRenderer>().material = WaterRight;
                            transform.Find("Cylinder_C").GetComponent<MeshRenderer>().material = WaterBa;
                            transform.Find("Val (3)").gameObject.SetActive(false);
                            transform.Find("Object028 (4)").gameObject.SetActive(true);
                        }
                    }
                    break;
                case "ButtonStand (4)":
                    if ((triggers[GetControlNumber(obj)]))
                    { 
                        triggers[GetControlNumber(obj)] = false;
                       
                        if (triggers[2])
                        {
                            if (triggers[5])
                            {
                                Debug.Log("6");
                                transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Val (4)").gameObject.SetActive(true);
                                transform.Find("Object028 (5)").gameObject.SetActive(false);
                                
                                
                                
                            }
                            else
                            {
                                
                                transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (17)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Val (6)").gameObject.SetActive(true);
                                transform.Find("Object028 (7)").gameObject.SetActive(false);
                                transform.Find("Val (4)").gameObject.SetActive(true);
                                transform.Find("Object028 (5)").gameObject.SetActive(false);
                                
                                
                            }
                        }
                        
                    }
                    else
                    {
                        if (triggers[2])
                        {
                            if (triggers[5])
                            {
                                Debug.Log("6");
                                transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Val (4)").gameObject.SetActive(false);
                                transform.Find("Object028 (5)").gameObject.SetActive(true);
                                triggers[GetControlNumber(obj)] = true;
                            }
                            else
                            {
                              
                                transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Val (4)").gameObject.SetActive(false);
                                transform.Find("Object028 (5)").gameObject.SetActive(true);
                                triggers[GetControlNumber(obj)] = true;
                                
                            }
                        }
                        
                    }

                    break;
                case "ButtonStand (5)":
                    if ((triggers[GetControlNumber(obj)]) )
                    {
                        if (triggers[3])
                        {
                            if (triggers[4])
                            {
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Val (5)").gameObject.SetActive(true);
                                transform.Find("Object028 (6)").gameObject.SetActive(false);
                                triggers[GetControlNumber(obj)] = false;
                            }
                            else
                            {
                                transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Val (4)").gameObject.SetActive(true);
                                transform.Find("Object028 (5)").gameObject.SetActive(false);
                                triggers[GetControlNumber(obj)] = false;
                                transform.Find("Tube (17)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Val (5)").gameObject.SetActive(true);
                                transform.Find("Object028 (6)").gameObject.SetActive(false);
                            }

                           
                        }
                    }
                    else
                    {
                        if (triggers[3])
                        {
                            if (triggers[4])
                            {
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Val (5)").gameObject.SetActive(false);
                                transform.Find("Object028 (6)").gameObject.SetActive(true);
                                triggers[GetControlNumber(obj)] = true;
                            }
                            else
                            {

                                transform.Find("Tube (13)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Tube (16)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (15)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Val (5)").gameObject.SetActive(false);
                                transform.Find("Object028 (6)").gameObject.SetActive(true);
                                triggers[GetControlNumber(obj)] = true;
                            }
                        }
                    }

                    break;
                case "ButtonStand (6)":
                    if ((triggers[GetControlNumber(obj)]))
                    {
                        triggers[GetControlNumber(obj)] = false;

                        if (triggers[2])
                        {
                            if (triggers[7])
                            {
                                transform.Find("Tube (18)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (19)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Cylinder.054_C").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Val (7)").gameObject.SetActive(true);
                                transform.Find("Object028 (8)").gameObject.SetActive(false);

                            }
                            else
                            {
                                transform.Find("Tube (18)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (19)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (23)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (21)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (26)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (27)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Cylinder.054_C").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Val (7)").gameObject.SetActive(false);
                                transform.Find("Object028 (8)").gameObject.SetActive(true);
                            }
                        } 
                    }

                    else
                    {
                        if (triggers[2])
                        {
                            if (triggers[7])
                            {
                                transform.Find("Tube (18)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (19)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Cylinder.054_C").GetComponent<MeshRenderer>().material = Manometer;
                                transform.Find("Val (7)").gameObject.SetActive(false);
                                transform.Find("Object028 (8)").gameObject.SetActive(true);
                            }
                            else
                            {

                                transform.Find("Tube (18)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (19)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Tube (23)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Tube (21)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (26)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (27)").GetComponent<MeshRenderer>().material = WaterRight;
                                
                                transform.Find("Cylinder.054_C").GetComponent<MeshRenderer>().material = Manometer;
                                transform.Find("Val (7)").gameObject.SetActive(false);
                                transform.Find("Object028 (8)").gameObject.SetActive(true);
                            }

                            triggers[GetControlNumber(obj)] = true;
                        }
                        
                    }


                    break;
                case "ButtonStand (7)":
                
                    if ((triggers[GetControlNumber(obj)]))
                    { 
                        triggers[GetControlNumber(obj)] = false;
                        Debug.Log("3.7");
                        if (triggers[3])
                        {
                           
                            if (triggers[6])
                            {
                                transform.Find("Tube (22)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (24)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Cylinder.055_C").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Val (8)").gameObject.SetActive(true);
                                transform.Find("Object028 (9)").gameObject.SetActive(false);
                            }
                            else
                            {
                                transform.Find("Tube (22)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (24)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (23)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (21)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (26)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Tube (27)").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Cylinder.055_C").GetComponent<MeshRenderer>().material = Default;
                                transform.Find("Val (8)").gameObject.SetActive(true);
                                transform.Find("Object028 (9)").gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        if (triggers[3])
                        {
                            triggers[GetControlNumber(obj)] = true;
                            if (triggers[6])
                            {
                                transform.Find("Tube (22)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (24)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (23)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Cylinder.055_C").GetComponent<MeshRenderer>().material = Manometer;
                                transform.Find("Val (8)").gameObject.SetActive(false);
                                transform.Find("Object028 (9)").gameObject.SetActive(true);
                            }
                            else
                            {
                                transform.Find("Tube (22)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (24)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (23)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (25)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (20)").GetComponent<MeshRenderer>().material = WaterLeft;
                                transform.Find("Tube (21)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (26)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Tube (27)").GetComponent<MeshRenderer>().material = WaterRight;
                                transform.Find("Cylinder.055_C").GetComponent<MeshRenderer>().material = Manometer;
                                transform.Find("Val (8)").gameObject.SetActive(false);
                                transform.Find("Object028 (9)").gameObject.SetActive(true);
                            }
                        }
                    }

                    break;
                case "ButtonStand (8)":
                    if ((triggers[GetControlNumber(obj)])) 
                    {
                        triggers[GetControlNumber(obj)] = false;
                        transform.Find("Tube (17)").GetComponent<MeshRenderer>().material = Default;
                        transform.Find("Val (6)").gameObject.SetActive(true);
                        transform.Find("Object028 (7)").gameObject.SetActive(false);
                    }
                    else
                    {
                        
                        if ((triggers[4])||(triggers[5]))
                        {
                            transform.Find("Tube (17)").GetComponent<MeshRenderer>().material = WaterRight;
                            transform.Find("Val (6)").gameObject.SetActive(false);
                            transform.Find("Object028 (7)").gameObject.SetActive(true);
                            triggers[GetControlNumber(obj)] = true;
                        }
                        
                    }

                    break;
            }
        }

        private int GetControlNumber(GameObject obj)
        {
            for (int i = 0; i < interactables.Count; i++)
            {
                if (obj == interactables[i]) return i;
            }

            return -1;
        }
    }
}