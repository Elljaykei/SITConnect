# SITConnect - Application Security Assignment
---

This README is the report for submission.

This Repository contains the source code required for submission.

Done by: Lim Jing Kai, IT2005 NYP


### Registration Page
---

**Set Strong password**

- Perform password complexity checks.
  - Server-based checks will reject registration if **requirements** are not met.
  - Client-based checks have all fields set as required, unless all filled, cannot be submitted.
  - **Requirements**: (Min 12 chars,
Use combination of lower-case, upper-case, Numbers
and special characters)
- Client-based tells user what is lacking and how to make the password stronger.

**Securing user data and passwords**

- Password is hashed using SHA512
  - Password has a salt prepended and hashed
  - Salt is randomly generated and stored alongside the PasswordHash
- Card number is encrypted using AES (Rijndael)
  - Card is encrypted using randomly generated IV and Key
  - IV, key and Card number is stored after being converted to Base64String

**Session**

- Session is created for Login and Registration (For Verification)
- AuthTokenVerification created with session and cookie for verification page.
- Normal AuthToken created after login.
- Session time-out is set to 1 minutes of idle.
- User is redirected to /UserDetails page after login.
- User is redirected to /Login if user is not logged in and tried to access other pages through url.

### Login Page
---
**Login/Logout**

- User can only login after registration and verification
- Account will be locked out after 3 login failures.
  - An attempt is regarded as an instance in which the user uses the same email and wrong password.
  - The code checks the auditlog for entries in the past 3 minutes.
  - If there are >=3 entries of "Failed Attempted Login" account remains locked out until there are <3 entries in the past 3 minutes.
- Audit log will be updated at login, password change and log out.

**Anti-bot**

- Google reCaptcha v3 enabled on /Login

### General
---
**Proper Input Validation**

- No direct SQL queries were used. Preventing SQLi.
- AuthTokenVerification creation. Preventing XSS.
- Verification via email is present
- Client and server input validation present
  - Password must adhere to a certain set of requirements if not it will be rejected.
  - Email must  be a legitimate email.

**Proper Error handling**

- Errors 403, 404 and 500 are specifically Handled.
- Other errors are handled by GenericError
- **Test cases**
  - /UserDetails.aspx (When not authenticated) - 403
  - /1.aspx - 404

**Software Testing - Source code analysis**

- CodeQL by Github present (0 Security Warnings left).

**Advanced Features**

- Account will be unlocked if in the last minute there are less than 3 failed login attempts..
- Password may be changed.
- Password cannot be same as last two passwords.
- Password age:
  - Cannot be changed for 1 minute since the last change.
  - Must be changed after 3 mins since the last password change.
    - User redirected to /PasswordChange upon login.
- 2FA present.
  - Randomised verification code is sent to the user's entered email address.
  - Each code expires after 5 mins.
