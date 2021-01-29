using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagixInterface : MonoBehaviour
{
    //User's hand
    public Transform Hand;

    //Used for visually debugging the positions after translation to XY
    public GameObject DebugMarker;
    private List<GameObject> markers;

    //Used for manual rotation detection
    public Transform TrackedObject;
    public Transform canvas;

    //Used for automatic rotation detection
    private GameObject plane;
    public GameObject Plane;

    //Used for rotation
    private Vector3[] truePositions;
    private Vector3 trueRotation;
    private Quaternion Rotation;
    private Vector3 middlePosition;

    //Used for spelldetection
    private float maxWidth, maxHeight, minWidth, minHeight;
    private float height, width;

    private bool release = true;
    private bool move;
    public void MoveCanvas()
    {
        canvas.position = TrackedObject.position;
        Vector3 ToRotate = new Vector3(TrackedObject.eulerAngles.x, TrackedObject.eulerAngles.y, 0);
        canvas.rotation = Quaternion.identity;
        canvas.Rotate(ToRotate);
        move = true;
    }

    public void StopMovingCanvas()
    {
        move = false;
    }

    public void Update()
    {
        if (move)
        {
            MoveCanvas();
        }
    }

    public void Run(List<Vector3> positions)
    {
        //CleanMarkers(); //Used for debugging
        CleanMarkers();
        //GetTrueRotation();
        InitializeValues(positions.Count);
        //AlignPlane(positions); //Used for automatic rotation detection
        //truePositions = TranslateRotation(positions);
        truePositions = RotateLikeABadass(positions);
        CleanMarkers();
        truePositions = MoveToZero(truePositions);
        CheckSpell();
        canvas.rotation = Rotation;
        //Show(); //Used for debugging
    }

    private void GetTrueRotation()
    {
        //https://answers.unity.com/questions/1589025/how-to-get-inspector-rotation-values.html
        trueRotation.x = canvas.eulerAngles.x - Mathf.CeilToInt(canvas.eulerAngles.x / 360f) * 360f;
        if (trueRotation.x > 0)
            trueRotation.x += 360f;
        trueRotation.y = canvas.eulerAngles.y - Mathf.CeilToInt(canvas.eulerAngles.y / 360f) * 360f;
        if (trueRotation.y > 0)
            trueRotation.y += 360f;
        trueRotation.z = canvas.eulerAngles.z - Mathf.CeilToInt(canvas.eulerAngles.z / 360f) * 360f;
        if (trueRotation.z > 0)
            trueRotation.z += 360f;
        Debug.Log("Before: " + trueRotation);
        RotationToRads();
        InvertRotation();
        Debug.Log("After: " + trueRotation);
    }

    private void RotationToRads()
    {
        //Get rotation in rads
        trueRotation.x = trueRotation.x * Mathf.Deg2Rad;
        trueRotation.y = trueRotation.y * Mathf.Deg2Rad;
        trueRotation.z = trueRotation.z * Mathf.Deg2Rad;
    }

    private void InvertRotation()
    {
        //Invert the rotation
        trueRotation.x = trueRotation.x * -1;
        trueRotation.y = trueRotation.y * -1;
        trueRotation.z = trueRotation.z * -1;
    }

    private void CleanMarkers()
    {
        //Delete old debug markers
        if (markers != null && markers.Count > 0)
        {
            foreach (GameObject mark in markers)
            {
                Destroy(mark);
            }
            markers.Clear();
        }
    }

    private void InitializeValues(int positions)
    {
        truePositions = new Vector3[positions];
        middlePosition = Vector3.zero;
        markers = new List<GameObject>();
        maxWidth = 0;
        maxHeight = 0;
    }

    private void AlignPlane(List<GameObject> positions)
    {
        Vector3 midPoint = Vector3.zero;
        foreach (GameObject pos in positions)
        {
            midPoint += pos.transform.position;
        }
        plane.transform.position = midPoint / positions.Count;

        Quaternion[] rotations = new Quaternion[positions.Count];
        Quaternion rotation = Quaternion.identity;
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 direction;
            direction = positions[i].transform.position - plane.transform.position;
            rotations[i] = Quaternion.LookRotation(direction);

            rotations[i].eulerAngles = rotations[i].eulerAngles / positions.Count;

            //plane.transform.rotation = plane.transform.rotation * rotations[i];
            rotation = rotation * rotations[i];
        }
        rotation.z = 0;
        plane.transform.rotation = plane.transform.rotation * rotation;
    }

    private Vector3[] RotateLikeABadass(List<Vector3> positions)
    {
        foreach (Vector3 pos in positions)
        {
            markers.Add(Instantiate(DebugMarker, pos, Quaternion.identity, canvas));
        }
        Rotation = canvas.rotation;
        canvas.rotation = Quaternion.identity;
        Vector3[] translatedPositions = new Vector3[positions.Count];
        int x = 0;
        foreach (GameObject marker in markers)
        {
            translatedPositions[x] = marker.transform.position;
            x++;
        }
        return translatedPositions;
    }

    #region Old TranslateRotations
    private Vector3[] TranslateRotation(List<Vector3> positions)
    {
        Debug.Log("V3 list to V3 array");
        //Get middle position
        foreach (Vector3 pos in positions)
        {
            middlePosition.x += pos.x;
            middlePosition.y += pos.y;
            middlePosition.z += pos.z;
        }
        middlePosition.x = middlePosition.x / positions.Count;
        middlePosition.y = middlePosition.y / positions.Count;
        middlePosition.z = middlePosition.z / positions.Count;

        //Rotate positions around middle position
        int x = 0;
        Vector3[] truePos = new Vector3[positions.Count];
        foreach (Vector3 pos in positions)
        {
            #region
            Matrix4x4 position = new Matrix4x4(new Vector4(1, 0, 0, pos.x - middlePosition.x),
                                               new Vector4(0, 1, 0, pos.y - middlePosition.y),
                                               new Vector4(0, 0, 1, pos.z - middlePosition.z),
                                               new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetX = new Matrix4x4(new Vector4(1, 0, 0, 0),
                                              new Vector4(0, Mathf.Cos(trueRotation.x), -Mathf.Sin(trueRotation.x), 0),
                                              new Vector4(0, Mathf.Sin(trueRotation.x), Mathf.Cos(trueRotation.x), 0),
                                              new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetY = new Matrix4x4(new Vector4(Mathf.Cos(trueRotation.y), 0, Mathf.Sin(trueRotation.y), 0),
                                              new Vector4(0, 1, 0, 0),
                                              new Vector4(-Mathf.Sin(trueRotation.y), 0, Mathf.Cos(trueRotation.y), 0),
                                              new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetZ = new Matrix4x4(new Vector4(Mathf.Cos(trueRotation.z), -Mathf.Sin(trueRotation.z), 0, 0),
                                              new Vector4(Mathf.Sin(trueRotation.z), Mathf.Cos(trueRotation.z), 0, 0),
                                              new Vector4(0, 0, 1, 0),
                                              new Vector4(0, 0, 0, 1));

            position = position * offsetX * offsetY * offsetZ;

            truePos[x] = new Vector3(position.m30 + middlePosition.x, position.m31 + middlePosition.y, 0);
            //truePositions[x] = new Vector3(position.m30 + middlePosition.x, position.m31 + middlePosition.y, 0);
            #endregion
            #region
            //GameObject marker = new GameObject();
            //marker.transform.position = pos;
            //marker.transform.RotateAround(middlePosition, Vector3.forward, -canvas.eulerAngles.x);
            //marker.transform.RotateAround(middlePosition, Vector3.right, -canvas.eulerAngles.y);
            //marker.transform.RotateAround(middlePosition, Vector3., -canvas.eulerAngles.z);
            //truePositions[x] = marker.transform.position;
            //truePositions[x].z = 0;
            #endregion
            #region
            //truePositions[x] = Quaternion.Euler(canvas.rotation.eulerAngles) * (pos - middlePosition) + middlePosition;
            //truePositions[x].z = 0;
            //Matrix4x4 position = new Matrix4x4(new Vector4(1, 0, 0, truePositions[x].x),
            //                       new Vector4(0, 1, 0, truePositions[x].y),
            //                       new Vector4(0, 0, 1, truePositions[x].z),
            //                       new Vector4(0, 0, 0, 1));

            //Matrix4x4 offsetX = new Matrix4x4(new Vector4(1, 0, 0, 0),
            //                                  new Vector4(0, Mathf.Cos(Mathf.PI), -Mathf.Sin(Mathf.PI), 0),
            //                                  new Vector4(0, Mathf.Sin(Mathf.PI), Mathf.Cos(Mathf.PI), 0),
            //                                  new Vector4(0, 0, 0, 1));

            //position = position * offsetX;
            //truePositions[x] = new Vector3(position.m30, position.m31, 0);
            #endregion

            x++;
        }
        return truePos;
    }

    private Vector3[] TranslateRotation(Vector3[] positions)
    {
        Debug.Log("V3 array to V3 array");
        //Get middle position
        foreach (Vector3 pos in positions)
        {
            middlePosition.x += pos.x;
            middlePosition.y += pos.y;
            middlePosition.z += pos.z;
        }
        middlePosition.x = middlePosition.x / positions.Length;
        middlePosition.y = middlePosition.y / positions.Length;
        middlePosition.z = middlePosition.z / positions.Length;

        //Rotate positions around middle position
        int x = 0;
        Vector3[] truePos = new Vector3[positions.Length];
        foreach (Vector3 pos in positions)
        {
            #region
            Matrix4x4 position = new Matrix4x4(new Vector4(1, 0, 0, pos.x - middlePosition.x),
                                               new Vector4(0, 1, 0, pos.y - middlePosition.y),
                                               new Vector4(0, 0, 1, pos.z - middlePosition.z),
                                               new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetX = new Matrix4x4(new Vector4(1, 0, 0, 0),
                                              new Vector4(0, Mathf.Cos(trueRotation.x), -Mathf.Sin(trueRotation.x), 0),
                                              new Vector4(0, Mathf.Sin(trueRotation.x), Mathf.Cos(trueRotation.x), 0),
                                              new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetY = new Matrix4x4(new Vector4(Mathf.Cos(trueRotation.y), 0, Mathf.Sin(trueRotation.y), 0),
                                              new Vector4(0, 1, 0, 0),
                                              new Vector4(-Mathf.Sin(trueRotation.y), 0, Mathf.Cos(trueRotation.y), 0),
                                              new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetZ = new Matrix4x4(new Vector4(Mathf.Cos(trueRotation.z), -Mathf.Sin(trueRotation.z), 0, 0),
                                              new Vector4(Mathf.Sin(trueRotation.z), Mathf.Cos(trueRotation.z), 0, 0),
                                              new Vector4(0, 0, 1, 0),
                                              new Vector4(0, 0, 0, 1));

            position = position * offsetX * offsetY * offsetZ;

            truePos[x] = new Vector3(position.m30 + middlePosition.x, position.m31 + middlePosition.y, 0);
            //truePositions[x] = new Vector3(position.m30 + middlePosition.x, position.m31 + middlePosition.y, 0);
            #endregion
            #region
            //GameObject marker = new GameObject();
            //marker.transform.position = pos;
            //marker.transform.RotateAround(middlePosition, Vector3.forward, -canvas.eulerAngles.x);
            //marker.transform.RotateAround(middlePosition, Vector3.right, -canvas.eulerAngles.y);
            //marker.transform.RotateAround(middlePosition, Vector3., -canvas.eulerAngles.z);
            //truePositions[x] = marker.transform.position;
            //truePositions[x].z = 0;
            #endregion
            #region
            //truePositions[x] = Quaternion.Euler(canvas.rotation.eulerAngles) * (pos - middlePosition) + middlePosition;
            //truePositions[x].z = 0;
            //Matrix4x4 position = new Matrix4x4(new Vector4(1, 0, 0, truePositions[x].x),
            //                       new Vector4(0, 1, 0, truePositions[x].y),
            //                       new Vector4(0, 0, 1, truePositions[x].z),
            //                       new Vector4(0, 0, 0, 1));

            //Matrix4x4 offsetX = new Matrix4x4(new Vector4(1, 0, 0, 0),
            //                                  new Vector4(0, Mathf.Cos(Mathf.PI), -Mathf.Sin(Mathf.PI), 0),
            //                                  new Vector4(0, Mathf.Sin(Mathf.PI), Mathf.Cos(Mathf.PI), 0),
            //                                  new Vector4(0, 0, 0, 1));

            //position = position * offsetX;
            //truePositions[x] = new Vector3(position.m30, position.m31, 0);
            #endregion

            x++;
        }
        return truePos;
    }

    private Vector3[] TranslateRotation(List<Vector2> positions)
    {
        Debug.Log("V2 list to V3 array");
        //Get middle position
        foreach (Vector3 pos in positions)
        {
            middlePosition.x += pos.x;
            middlePosition.y += pos.y;
            middlePosition.z += pos.z;
        }
        middlePosition.x = middlePosition.x / positions.Count;
        middlePosition.y = middlePosition.y / positions.Count;
        middlePosition.z = middlePosition.z / positions.Count;

        //Rotate positions around middle position
        int x = 0;
        Vector3[] truePos = new Vector3[positions.Count];
        foreach (Vector3 pos in positions)
        {
            #region
            Matrix4x4 position = new Matrix4x4(new Vector4(1, 0, 0, pos.x - middlePosition.x),
                                               new Vector4(0, 1, 0, pos.y - middlePosition.y),
                                               new Vector4(0, 0, 1, pos.z - middlePosition.z),
                                               new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetX = new Matrix4x4(new Vector4(1, 0, 0, 0),
                                              new Vector4(0, Mathf.Cos(trueRotation.x), -Mathf.Sin(trueRotation.x), 0),
                                              new Vector4(0, Mathf.Sin(trueRotation.x), Mathf.Cos(trueRotation.x), 0),
                                              new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetY = new Matrix4x4(new Vector4(Mathf.Cos(trueRotation.y), 0, Mathf.Sin(trueRotation.y), 0),
                                              new Vector4(0, 1, 0, 0),
                                              new Vector4(-Mathf.Sin(trueRotation.y), 0, Mathf.Cos(trueRotation.y), 0),
                                              new Vector4(0, 0, 0, 1));

            Matrix4x4 offsetZ = new Matrix4x4(new Vector4(Mathf.Cos(trueRotation.z), -Mathf.Sin(trueRotation.z), 0, 0),
                                              new Vector4(Mathf.Sin(trueRotation.z), Mathf.Cos(trueRotation.z), 0, 0),
                                              new Vector4(0, 0, 1, 0),
                                              new Vector4(0, 0, 0, 1));

            position = position * offsetX * offsetY * offsetZ;

            truePos[x] = new Vector3(position.m30 + middlePosition.x, position.m31 + middlePosition.y, 0);
            //truePositions[x] = new Vector3(position.m30 + middlePosition.x, position.m31 + middlePosition.y, 0);
            #endregion
            #region
            //GameObject marker = new GameObject();
            //marker.transform.position = pos;
            //marker.transform.RotateAround(middlePosition, Vector3.forward, -canvas.eulerAngles.x);
            //marker.transform.RotateAround(middlePosition, Vector3.right, -canvas.eulerAngles.y);
            //marker.transform.RotateAround(middlePosition, Vector3., -canvas.eulerAngles.z);
            //truePositions[x] = marker.transform.position;
            //truePositions[x].z = 0;
            #endregion
            #region
            //truePositions[x] = Quaternion.Euler(canvas.rotation.eulerAngles) * (pos - middlePosition) + middlePosition;
            //truePositions[x].z = 0;
            //Matrix4x4 position = new Matrix4x4(new Vector4(1, 0, 0, truePositions[x].x),
            //                       new Vector4(0, 1, 0, truePositions[x].y),
            //                       new Vector4(0, 0, 1, truePositions[x].z),
            //                       new Vector4(0, 0, 0, 1));

            //Matrix4x4 offsetX = new Matrix4x4(new Vector4(1, 0, 0, 0),
            //                                  new Vector4(0, Mathf.Cos(Mathf.PI), -Mathf.Sin(Mathf.PI), 0),
            //                                  new Vector4(0, Mathf.Sin(Mathf.PI), Mathf.Cos(Mathf.PI), 0),
            //                                  new Vector4(0, 0, 0, 1));

            //position = position * offsetX;
            //truePositions[x] = new Vector3(position.m30, position.m31, 0);
            #endregion

            x++;
        }
        return truePos;
    }
    #endregion

    private Vector3[] MoveToZero(Vector3[] positions)
    {
        //Get true middle position of the points.
        middlePosition = Vector3.zero;
        maxHeight = 0;
        minHeight = 0;
        maxWidth = 0;
        minWidth = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].x > maxWidth || i == 0)
            {
                maxWidth = positions[i].x;
            }
            if (positions[i].x < minWidth || i == 0)
            {
                minWidth = positions[i].x;
            }

            if (positions[i].y > maxHeight || i == 0)
            {
                maxHeight = positions[i].y;
            }
            if (positions[i].y < minHeight || i == 0)
            {
                minHeight = positions[i].y;
            }
        }
        middlePosition.x = (maxWidth + minWidth) / 2;
        middlePosition.y = (maxHeight + minHeight) / 2;
        middlePosition.z = 0;

        //Move positions to around 0 in world
        Vector3[] centeredPositions = new Vector3[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            centeredPositions[i].x = positions[i].x - middlePosition.x;
            centeredPositions[i].y = positions[i].y - middlePosition.y;
            centeredPositions[i].z = 0;
        }
        return centeredPositions;
    }

    private void CheckSpell()
    {
        //Get the true size between min to max of width and height.
        height = maxHeight - minHeight;
        width = maxWidth - minWidth;
        GameObject spell;
        if (height > width)
        {
            //Get length to add to positions to move them to around the center of the image.
            float centerLength = height / 2;
            spell = CheckEachPoint(centerLength);
        }
        else if (width >= height)
        {
            //Get length to add to positions to move them to around the center of the image.
            float centerLength = width / 2;
            spell = CheckEachPoint(centerLength);
        }
        else
        {
            Debug.LogError("Error with width or height. Height = " + height + ", Width = " + width);
            spell = null;
        }
        if (spell != null)
        {
           
            GameObject thisSpell = Instantiate(spell, Hand.position, Rotation);
            thisSpell.GetComponent<TriggerScript>().Initiate(Hand, this);
         
        }
    }


    public void Release()
    {
        if (release)
        {
            release = false;
        }
        else
        {
            release = true;
        }
        
    }

    public bool ReleaseSpell()
    {
        return release;
    }



    private GameObject CheckEachPoint(float centerLength)
    {
        int iCA = 0;
        int lastCA = 0;
        float ratio = 0;
        float misses = 0f;
        float gayMisses = 0f;
        float missRatio = 0f;
        bool success = false;
        Spell currentSpell = null;
        foreach (Spell spell in GetComponent<SpellLibrary>().spells)
        {
            //Reset values
            iCA = 0;
            lastCA = 0;
            ratio = 0;
            misses = 0f;
            gayMisses = 0f;
            missRatio = 0f;
            currentSpell = spell;
            ratio = spell.spellSprite.rect.height / height;
            //Check if the first position is near the first Control Area.
            int imagePositionX = (int)((truePositions[0].x + centerLength) * ratio);
            int imagePositionY = (int)((truePositions[0].y + centerLength) * ratio);
            if (Vector2.Distance(currentSpell.ControlAreas[iCA], new Vector2(imagePositionX, imagePositionY)) > 10f)
            {
                //If it isn't, check another spell.
                Debug.Log("Skipped spell: " + currentSpell.name);
                continue;
            }
            iCA++;
            success = true;

            #region Debug
            //Debug.Log("Width = " + maxWidth + ", Height = " + maxHeight);
            //Debug.Log("Spell image width = " + spell.spellSprite.rect.width + ", height = " + spell.spellSprite.rect.height);
            //Debug.Log("Ratio = " + ratio);
            #endregion
            #region WithBlackCheck
            for (int i = 1; i < truePositions.Length; i++)
            {
                imagePositionX = (int)((truePositions[i].x + centerLength) * ratio);
                imagePositionY = (int)((truePositions[i].y + centerLength) * ratio);
                if (currentSpell.spellSprite.texture.GetPixel(imagePositionX, imagePositionY) == new Color(1, 1, 1, 1))
                {
                    //Debug.Log("HIT");
                    if (iCA < currentSpell.ControlAreas.Count)
                    {
                        if (Vector2.Distance(currentSpell.ControlAreas[iCA], new Vector2(imagePositionX, imagePositionY)) < 10f)
                        {
                            Debug.Log("Hit ControlArea: " + iCA);
                            gayMisses = misses;
                            lastCA = i;
                            iCA++;
                        }
                    }
                }
                else
                {
                    // Debug.Log("MISS");
                    misses++;
                    missRatio = misses / truePositions.Length;
                    if (missRatio > 0.3f)
                    {
                        success = false;
                        if (currentSpell.succeedingSpells.Count > 0)
                        {
                            foreach (Successor successor in currentSpell.succeedingSpells)
                            {
                                Debug.Log("Checking to see if curreent ControlArea " + iCA + " is the one required for " + successor.ControlAreaID);
                                if (successor.ControlAreaID == iCA)
                                {
                                    Debug.Log("And it was!");
                                    misses = gayMisses;
                                    missRatio = misses / truePositions.Length;
                                    currentSpell = successor.succeedingSpell;
                                    i = lastCA;
                                    success = true;
                                    break;
                                }
                            }
                            if (success)
                                continue;
                            break;
                        }
                    }
                    #region Debug
                    //Debug.Log("Original position: " + truePositions[i].x + ", " + truePositions[i].y);
                    //Debug.Log("Translated image position: " + imagePositionX + ", " + imagePositionY);
                    #endregion
                }
            }
            Debug.Log("Ratio: " + missRatio + ", Misses: " + misses + ", Positions: " + truePositions.Length);
            #endregion

            #region Debug
            //Debug.Log("Original position: " + truePositions[i].x + ", " + truePositions[i].y);
            //Debug.Log("Translated image position: " + imagePositionX + ", " + imagePositionY);
            #endregion
            Debug.Log(currentSpell.name + " attempted.");
            if (currentSpell.ControlAreas.Count == iCA)
            {
                success = true;
                break;
            }
            break;
        }
        if (success)
        {
            if (currentSpell.ControlAreas.Count == iCA)
            {
                Debug.Log("SUCCESS!" + currentSpell.name);
                return currentSpell.Effect;
            }
            else
            {
                Debug.Log("PARTIAL!" + currentSpell.name);
                ShowSolution(currentSpell);
                return null;
            }
        }
        else
            Debug.Log("FAILURE!" + currentSpell.name);
        return null;
    }

    private void ShowSolution(Spell spell)
    {
        float ratio = 200f;
        Vector3[] positions = TranslateRotation(spell.ControlAreas);
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = new Vector3(positions[i].x / ratio, positions[i].y / ratio, positions[i].z / ratio);
        }
        positions = MoveToZero(positions);
        foreach (Vector3 pos in positions)
        {
            markers.Add(Instantiate(DebugMarker, pos + canvas.position, Quaternion.identity, canvas));
        }
    }

    private void Show()
    {
        foreach (Vector3 pos in truePositions)
            markers.Add(Instantiate(DebugMarker, pos, Quaternion.identity, canvas));
    }

}

