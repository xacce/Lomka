// using System;
// using System.Collections.Generic;
// using Unity.Mathematics;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEngine;
//
// namespace Lomka
// {
//     public class SphereKeyPoints : MonoBehaviour
//     {
//         public int longitudeSegments = 12; // Количество сегментов по долготе (phi)
//         public int latitudeSegments = 6; // Количество сегментов по широте (theta)
//         public float radius = 5f; // Радиус сферы
//         public GameObject objectPrefab; // Префаб объекта, который нужно расположить на вершинах
//         private List<GameObject> _gameObjects;
//         public int showforIndex;
//
//         public static int2 GetLongLatFromIndex(int index, int longMax, int latMax)
//         {
//             // if (index == 0) return int2.zero;
//             // var myLat =(int)math.ceil(index / ((float)longMax));
//             var myLat = index % longMax == 0 ? index / longMax + 1 : (int)math.ceil(index / ((float)longMax));
//             var myLong = index - (myLat - 1) * longMax;
//
//             return new int2(myLat, myLong);
//         }
//
//         public static int GetRotatedLong(int cL, int maxCl)
//         {
//             if (cL == maxCl) return 0;
//             if (cL < 0) return maxCl + cL;
//             return cL;
//         }
//
//         public static void GetNeighbours(int index, int longMax, int latMax, List<int> neighbours)
//         {
//             var northenIndex = longMax * (latMax - 1);
//             var southIndex = northenIndex + 1;
//             if (index == northenIndex)
//             {
//                 for (int i = 0; i < longMax; i++)
//                 {
//                     neighbours.Add(i);
//                 }
//                 return;
//             }
//             else if (index == southIndex)
//             {
//                 for (int i = (latMax-2)*longMax; i < northenIndex; i++)
//                 {
//                     neighbours.Add(i);
//                 }
//
//                 return;
//             }
//             var myCoordinate = GetLongLatFromIndex(index, longMax, latMax);
//             if (myCoordinate.x == 1)
//             {
//                 neighbours.Add(northenIndex); //add northen
//             }
//             else if (myCoordinate.x == latMax - 1)
//             {
//                 neighbours.Add(southIndex); //add south
//             }
//
//             for (int i = math.max(myCoordinate.x - 1, 1); i <= math.min(myCoordinate.x + 1, latMax - 1); i++)
//             {
//                 if (myCoordinate.x != i)
//                 {
//                     neighbours.Add((i - 1) * longMax + myCoordinate.y);
//                 }
//
//                 neighbours.Add((i - 1) * longMax + GetRotatedLong(myCoordinate.y - 1, longMax));
//                 neighbours.Add((i - 1) * longMax + GetRotatedLong(myCoordinate.y + 1, longMax));
//             }
//         }
//
//         void Start()
//         {
//             for (int lat = 1; lat < latitudeSegments; lat++) 
//             {
//                 float theta = Mathf.PI * lat / latitudeSegments;
//
//                 for (int lon = 0; lon < longitudeSegments; lon++)
//                 {
//                     float phi = 2 * Mathf.PI * lon / longitudeSegments;
//
//                     float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
//                     float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
//                     float z = radius * Mathf.Cos(theta);
//
//                     Vector3 pointPosition = new Vector3(x, y, z);
//                     var go = Instantiate(objectPrefab, pointPosition, Quaternion.identity);
//                     _gameObjects.Add(go);
//                 }
//             }
//
//             Vector3 northPole = new Vector3(0, 0, radius);
//             Instantiate(objectPrefab, northPole, Quaternion.identity);
//             _gameObjects.Add(Instantiate(objectPrefab, northPole, Quaternion.identity));
//             Vector3 southPole = new Vector3(0, 0, -radius);
//             _gameObjects.Add(Instantiate(objectPrefab, southPole, Quaternion.identity));
//         }
//
//
//         private void OnDrawGizmos()
//         {
//             List<Vector3> positions = new List<Vector3>();
//             List<int> neighbors = new List<int>();
//
//             Vector3 northPole = new Vector3(0, 0, radius);
//             var mycoord = GetLongLatFromIndex(showforIndex, longitudeSegments, latitudeSegments);
//
//
//             for (int lat = 1; lat < latitudeSegments; lat++) // Пропускаем первый и последний ряд для полюсов
//             {
//                 // Полярный угол theta: от 0 до PI (не включая 0 и PI, чтобы исключить полюса)
//                 float theta = Mathf.PI * lat / latitudeSegments;
//
//                 for (int lon = 0; lon < longitudeSegments; lon++)
//                 {
//                     // Азимутальный угол phi: от 0 до 2PI
//                     float phi = 2 * Mathf.PI * lon / longitudeSegments;
//
//                     // Преобразование в декартовы координаты
//                     float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
//                     float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
//                     float z = radius * Mathf.Cos(theta);
//
//                     // Позиция точки
//                     Vector3 pointPosition = new Vector3(x, y, z);
//                     // Создание объекта на вершине
//                     positions.Add(pointPosition);
//                     Handles.Label(pointPosition, $"Lat: {lat}, lon : {lon} ({positions.Count - 1})");
//                     if (lat == mycoord.x && lon == mycoord.y)
//                     {
//                         Gizmos.color = Color.green;
//                         Gizmos.DrawWireSphere(pointPosition, 0.25f);
//                         Gizmos.color = Color.white;
//                     }
//                 }
//             }
//
//             Vector3 southPole = new Vector3(0, 0, -radius);
//             positions.Add(northPole);
//             Handles.Label(northPole, $"First:{positions.Count - 1}");
//             positions.Add(southPole);
//
//             Handles.Label(southPole, $"Last:{positions.Count - 1}");
//             GetNeighbours(showforIndex, longitudeSegments, latitudeSegments, neighbors);
//             Gizmos.DrawWireSphere(positions[showforIndex], 0.15f);
//             Handles.Label(positions[showforIndex], $"\n\n\n{mycoord}");
//             for (int i = 0; i < neighbors.Count; i++)
//             {
//                 Gizmos.DrawLine(positions[showforIndex], positions[neighbors[i]]);
//             }
//         }
//     }
// }