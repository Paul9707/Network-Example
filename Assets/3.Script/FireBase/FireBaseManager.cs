using Firebase;
using Firebase.Extensions;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using Firebase.Database;
using Newtonsoft.Json;

public class FireBaseManager : MonoBehaviour
{
    #region Public Fields
    public static FireBaseManager Instance { get; private set; }

    public FirebaseApp App { get; private set; } // ���̾�̽� �⺻ ��(�⺻ ��ɵ�)
    public FirebaseAuth Auth { get; private set; } // ���� (�α���) ��� ����
    public FirebaseDatabase DB { get; private set; } // �����ͺ��̽� ��� ����
    

    // ���̾�̽� ���� �ʱ�ȭ �Ǿ� ��� �������� ����
    public bool IsInitialized { get; private set; } = false;
    #endregion

    public UserData userData;
    public DatabaseReference usersRef;
    #region event
    public event Action onInit; // ���̾�̽��� �ʱ�ȭ�Ǹ� ȣ��
    #endregion
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        InitializeAsync();
    }
    private void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(
            (Task<DependencyStatus> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"���̾���� �ʱ�ȭ ����: {task.Status}");
                }
                else if (task.IsCompleted)
                {
                    print($"���̾�̽� �ʱ�ȭ ����: {task.Status}");

                    if (task.Result == DependencyStatus.Available)
                    {
                        App = FirebaseApp.DefaultInstance;
                        Auth = FirebaseAuth.DefaultInstance;
                    }
                }
            }
            );
    }

    //async Ű���带 ���� �񵿱� ���α׷���
    private async void InitializeAsync()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status == DependencyStatus.Available)
        {
            // ���̾�̽� �ʱ�ȭ ����
            print($"���̾�̽� �ʱ�ȭ ����!");
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;
            IsInitialized = true;
            onInit?.Invoke();
        }
        else
        {
            // ���̾�̽� �ʱ�ȭ ����
            Debug.LogWarning($"���̾���� �ʱ�ȭ ����: {status}");
        }
        
    }

    public async void Login(string email, string pw, Action<FirebaseUser> callback = null)
    {
        var result = await Auth.SignInWithEmailAndPasswordAsync(email, pw);

        usersRef = DB.GetReference($"users/{result.User.UserId}");


        DataSnapshot userDataValues = await usersRef.GetValueAsync();

        if (userDataValues.Exists)
        {
            string json = userDataValues.GetRawJsonValue(); // ������ ��ü�� json���� ������
            var address = userDataValues.Child("address"); // �������� ���� ���۷���(������ ������)�� ������
            Debug.Log(json);
            if (address.Exists)
            {
                print($"�ּ� : {address.GetValue(false)}"); 
            }
            userData = JsonConvert.DeserializeObject<UserData>(json);

        }
        else 
        {
            FBPanelManager.Instance.Dialog("�α��� ������ ������ �ֽ��ϴ�.\n�����Ϳ� �����ϼ���.");
        }

        
        callback?.Invoke(result.User);
    }

    public async void Create(string email, string pw, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, pw);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            UserData userData = new UserData(result.User.UserId);

            string userDataJson = JsonConvert.SerializeObject(userData);

            await usersRef.SetRawJsonValueAsync(userDataJson);

            this.userData = userData;

            callback?.Invoke(result.User);
        }
        catch (FirebaseException fe)
        {
            Debug.LogError(fe.Message);
        }
    }

    public async void UpdateUser(string name, string pw, Action callback = null)
    {
        var profile = new UserProfile() { DisplayName = name };

        await Auth.CurrentUser.UpdateUserProfileAsync(profile);
        if(false == string.IsNullOrWhiteSpace(pw))
        await Auth.CurrentUser.UpdatePasswordAsync(pw);
        callback?.Invoke();

    }

    public void Logout() => Auth.SignOut();
    
}
