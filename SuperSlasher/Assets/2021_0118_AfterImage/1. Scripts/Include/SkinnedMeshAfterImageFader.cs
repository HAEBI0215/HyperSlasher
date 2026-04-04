using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-18 PM 7:06:31
// 작성자 : Rito

public sealed class SkinnedMeshAfterImageFader : AfterImageFaderBase
{
    private List<SkinnedMeshRenderer> TargetSmrList { get; set; }
    private List<MeshFilter> ChildrenFilterList { get; set; }

    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .
    public override void Setup(Array targetArray, AfterImageData data, AfterImageBase controller)
    {
        TargetTransformList = new List<Transform>();
        TargetSmrList = new List<SkinnedMeshRenderer>();

        ChildrenTransformList = new List<Transform>();
        ChildrenFilterList = new List<MeshFilter>();
        ChildrenRendererList = new List<MeshRenderer>();

        SkinnedMeshRenderer[] targetSmrs = targetArray as SkinnedMeshRenderer[];
        foreach (var smr in targetSmrs)
        {
            TargetTransformList.Add(smr.transform);
            TargetSmrList.Add(smr);
        }

        Data = data;
        Controller = controller;
        CurrentElapsedTime = 0f;

        CreateChildImages();
    }

    public override void WakeUp(in Color color)
{
    for (int i = 0; i < ChildrenFilterList.Count; i++)
    {
        // 1. 메쉬를 굽습니다. (이때 이미 캐릭터의 lossyScale이 반영된 크기로 구워짐)
        TargetSmrList[i].BakeMesh(ChildrenFilterList[i].mesh);

        // 2. [핵심] 잔상 오브젝트의 위치와 회전은 맞추되, 스케일은 1,1,1로 고정합니다.
        // BakeMesh가 이미 월드 스케일을 반영해서 메쉬를 생성하기 때문에 
        // 오브젝트 스케일까지 커지면 "스케일 x 스케일"이 되어 거대해지는 것입니다.
        ChildrenTransformList[i].position = TargetSmrList[i].transform.position;
        ChildrenTransformList[i].rotation = TargetSmrList[i].transform.rotation;
        ChildrenTransformList[i].localScale = Vector3.one; 
    }
    base.WakeUp(color);
}

protected override void CreateChildImages()
{
    for (int i = 0; i < TargetSmrList.Count; i++)
    {
        GameObject instanceGo = new GameObject("Image");
        Transform instanceTr = instanceGo.transform;

        // [핵심] 잔상 컨테이너가 캐릭터의 자식이라면 부모 스케일 영향을 받지 않게 Root로 뺍니다.
        // 만약 부모가 100배라면 자식인 잔상도 100배가 되기 때문입니다.
        instanceTr.SetParent(transform); 

        var filter = instanceGo.AddComponent<MeshFilter>();
        var renderer = instanceGo.AddComponent<MeshRenderer>();

        // BakeMesh를 담을 빈 메쉬를 미리 할당
        filter.mesh = new Mesh(); 

        renderer.material = Data.Mat;

        ChildrenRendererList.Add(renderer);
        ChildrenFilterList.Add(filter);
        ChildrenTransformList.Add(instanceTr);
    }
}

    #endregion
}