using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteSelector : MonoBehaviour
{
    public Sprite spU, spD, spL, spR, spUD, spUL, spUR, spDL, spDR, spLR, spUDL, spUDR, spULR, spDLR, spUDLR;
    public bool up, down, left, right;
    public int type; //0 = normal, 1 = start, 2 = end
    public Color normalColor, startColor, endColor;
    Color mainColor;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mainColor = normalColor;
        PickSprite();
        PickColor();
    }

    //ew, make it better
    void PickSprite()
    {
        if (up){
            if (down){
                if (right){
                    if (left){
                        sr.sprite = spUDLR;
                    }else{
                        sr.sprite = spUDR;
                    }
                }else if (left){
                    sr.sprite = spUDL;
                }else{
                    sr.sprite = spUD;
                }
            }else{
                if (right){
                    if (left){
                        sr.sprite = spULR;
                    }else{
                        sr.sprite = spUR;
                    }
                }else if (left){
                    sr.sprite = spUL;
                }else{
                    sr.sprite = spU;
                }
            }
            return;
        }
        if (down){
            if (right){
                if(left){
                    sr.sprite = spDLR;
                }else{
                    sr.sprite = spDR;
                }
            }else if (left){
                sr.sprite = spDL;
            }else{
                sr.sprite = spD;
            }
            return;
        }
        if (right){
            if (left){
                sr.sprite = spLR;
            }else{
                sr.sprite = spR;
            }
        }else{
            sr.sprite = spL;
        }
    }

    void PickColor()
    {
        if (type == 0)
        {
            mainColor = normalColor;
        }
        else if (type == 1)
        {
            mainColor = startColor;
        }
        else if (type == 2)
        {
            mainColor = endColor;
        }
        sr.color = mainColor;
    }
}