# SessionDemo - ASP.NET Core MVC Session Management

## 📌 Project Overview
SessionDemo is an ASP.NET Core MVC project that demonstrates **session persistence, automatic session extension, and user idle detection**. The system ensures users remain logged in while actively interacting and prompts them to extend their session if they are idle. If the user does not respond to the warning, they are automatically logged out.

---
## 🚀 Features
✅ **Persistent Authentication Cookies** - Ensures user sessions are retained even after closing the browser.✅ **Automatic Session Extension** - Extends session timeout automatically when the user interacts.✅ **Idle Detection & Warning Popup** - Displays a session expiration warning after inactivity.✅ **Auto-Logout on Inactivity** - Logs out users if they do not respond within 15 seconds of the warning.✅ **Customizable Session Timeout** - Can be adjusted in `Program.cs`.

---
## 🛠 Project Setup
### **1️⃣ Clone the Repository**
```
git clone https://github.com/your-repo/SessionDemo.git
cd SessionDemo
```

### **2️⃣ Open in Visual Studio 2022**
1. Open `SessionDemo.sln` in **Visual Studio 2022**.
2. Ensure you have the latest **.NET 6 or 7 SDK** installed.

### **3️⃣ Run the Application**
Press `F5` or use the command:
```
dotnet run
```

---
## 🛠 Key Configuration Changes
### **1️⃣ Modify `Program.cs` for Persistent Sessions**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // Session expires after 10 mins
        options.SlidingExpiration = true; // Extends session on user activity
    });
```

### **2️⃣ Implement `ExtendSession` Method in `AccountController.cs`**
```csharp
[HttpPost]
public IActionResult ExtendSession()
{
    if (User.Identity is ClaimsIdentity identity)
    {
        var claims = identity.Claims.ToList();
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);

        return Ok(new { message = "Session extended" });
    }
    return Unauthorized();
}
```

### **3️⃣ Add JavaScript for Automatic Session Extension & Logout**
Modify `_Layout.cshtml` to include the following script:
```html
<script>
    var sessionTimeout = 600000; // 10 minutes
    var warningTime = sessionTimeout - 60000; // Show warning 1 min before expiration
    var idleTime = 0;
    var logoutTimeout, warningTimeout;
    var sessionExtended = false;

    function resetIdleTimer() {
        idleTime = 0;
        if (!sessionExtended) {
            extendUserSession();
            sessionExtended = true;
        }
    }

    function checkIdleTime() {
        idleTime += 1000;
        if (idleTime >= warningTime && !document.getElementById("sessionWarning")) {
            showWarning();
        }
    }

    function showWarning() {
        let dialog = document.createElement("div");
        dialog.innerHTML = `
            <div id="sessionWarning" style="position: fixed; bottom: 20px; right: 20px; padding: 15px; background: orange; border-radius: 5px;">
                <p>Your session is about to expire! Extend session?</p>
                <button onclick="extendUserSession()">Extend Session</button>
                <button onclick="forceLogout()">Logout</button>
            </div>
        `;
        document.body.appendChild(dialog);
        logoutTimeout = setTimeout(() => { forceLogout(); }, 15000);
    }

    function extendUserSession() {
        fetch('/Account/ExtendSession', { method: 'POST' })
            .then(response => {
                if (response.ok) {
                    sessionExtended = true;
                    clearTimeout(logoutTimeout);
                    if (document.getElementById("sessionWarning")) {
                        document.getElementById("sessionWarning").remove();
                    }
                    idleTime = 0;
                }
            });
    }

    function forceLogout() {
        window.location.href = "/Account/Logout";
    }

    document.addEventListener("mousemove", resetIdleTimer);
    document.addEventListener("keypress", resetIdleTimer);
    document.addEventListener("click", resetIdleTimer);
    document.addEventListener("scroll", resetIdleTimer);

    setInterval(checkIdleTime, 1000);
</script>
```

---
## 🔍 **How to Test**
1. **Login** at `http://localhost:xxxx/Account/Login`.
2. **Use the system** - Session should extend automatically.
3. **Stay idle for 9 minutes** - A warning will appear.
4. **Ignore the warning for 15 seconds** - You will be automatically logged out.
5. **Click "Extend Session"** - The session will refresh, and you will stay logged in.

---
## 📌 Future Improvements
🚀 **Add a countdown timer to the warning popup.**  
🚀 **Improve UI with Bootstrap modals for better user experience.**  
🚀 **Store session extension logs in the database for tracking.**

---
## 📝 License
This project is open-source and available for modification and improvement.

---
## 💬 Need Help?
For issues or improvements, feel free to create an issue or submit a pull request. 🚀
