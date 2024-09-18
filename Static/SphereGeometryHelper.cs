using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Lomka.Static
{
    public static class SphereGeometryHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 GetLongLatFromIndex(int index, int longMax, int latMax)
        {
            // if (index == 0) return int2.zero;
            // var myLat =(int)math.ceil(index / ((float)longMax));
            var myLat = index % longMax == 0 ? index / longMax + 1 : (int)math.ceil(index / ((float)longMax));
            var myLong = index - (myLat - 1) * longMax;

            return new int2(myLat, myLong);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRotatedLong(int cL, int maxCl)
        {
            if (cL == maxCl) return 0;
            if (cL < 0) return maxCl + cL;
            return cL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildSphere(int latitudeSegments, int longitudeSegments, float radius, ref NativeList<float3> positions)
        {
            for (int lat = 1; lat < latitudeSegments; lat++)
            {
                float theta = Mathf.PI * lat / latitudeSegments;

                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    float phi = 2 * Mathf.PI * lon / longitudeSegments;

                    float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
                    float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
                    float z = radius * Mathf.Cos(theta);//todo move to unity math

                    float3 pointPosition = new Vector3(x, y, z);
                    positions.Add(pointPosition);
                }
            }

            Vector3 northPole = new Vector3(0, 0, radius);
            positions.Add(northPole);
            Vector3 southPole = new Vector3(0, 0, -radius);
            positions.Add(southPole);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetNeighbours(int index, int longMax, int latMax,ref NativeList<int> neighbours)
        {
            var northenIndex = longMax * (latMax - 1);
            var southIndex = northenIndex + 1;
            if (index == northenIndex)
            {
                for (int i = 0; i < longMax; i++)
                {
                    neighbours.Add(i);
                }

                return;
            }
            else if (index == southIndex)
            {
                for (int i = (latMax - 2) * longMax; i < northenIndex; i++)
                {
                    neighbours.Add(i);
                }

                return;
            }

            var myCoordinate = GetLongLatFromIndex(index, longMax, latMax);
            if (myCoordinate.x == 1)
            {
                neighbours.Add(northenIndex); //add northen
            }
            else if (myCoordinate.x == latMax - 1)
            {
                neighbours.Add(southIndex); //add south
            }

            for (int i = math.max(myCoordinate.x - 1, 1); i <= math.min(myCoordinate.x + 1, latMax - 1); i++)
            {
                if (myCoordinate.x != i)
                {
                    neighbours.Add((i - 1) * longMax + myCoordinate.y);
                }

                neighbours.Add((i - 1) * longMax + GetRotatedLong(myCoordinate.y - 1, longMax));
                neighbours.Add((i - 1) * longMax + GetRotatedLong(myCoordinate.y + 1, longMax));
            }
        }
    }
}