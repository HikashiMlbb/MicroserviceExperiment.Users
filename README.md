# Documentation

## Environment
Before running, please, fill environment arguments.
```.env
CONNECTION_STRING = "* PostgreSQL Connection String *"
AUTHORIZATION_TOKEN__ISSUER = "* JSON Web Token (JWT) Issuer Name *"
AUTHORIZATION_TOKEN__AUDIENCE = "* JSON Web Token (JWT) Audience Name *"
AUTHORIZATION_TOKEN__EXPIRATION = "* JSON Web Token (JWT) Expiration Time (.NET TimeSpan Convention) *"
AUTHORIZATION_TOKEN__KEY = "* JSON Web Token (JWT) Secret Key (Minimum 384 bits) *"
RESET_TOKEN__URL = "* The URL to WebSite where email with reset password token links to *"
RESET_TOKEN__EXPIRATION = "* The Reset Password Token Expiration (Minimum 1 min, Maximum 1 day) *"
RABBIT_MQ__CONNECTION_STRING = "* RabbitMQ Connection String *"
RABBIT_MQ__QUEUE_NAME = "* RabbitMQ Notification Queue Name *"
CACHE_CONNECTION = "* Redis Cache Connection String *"
```

## Endpoints

### User Endpoints

#### `POST /api/users/sign-up:`
```
Accepts: application/json
Body:

{
  "email": string (email),
  "username": string,
  "password": string
}

Returns:

HTTP 200 (OK) -> Successfully signed up. + (authorization token string in response)
HTTP 400 (BadRequest) -> Invalid Email, Username or Password
HTTP 409 (Conflict) -> User with such Email or Username already exists.

Username requirements: min 4, max 20 symbols; only ASCII letters or digits.
Password requirements: min 5, max 19 symbols.
```

---

#### `POST /api/users/sign-in:`
```
Accepts: application/json
Body:

{
  "username": string,
  "password": string
}

Returns:

HTTP 200 (OK) -> Successfully signed in. + (authorization token string in response)
HTTP 401 (Unauthorized) -> Signing in is failed.
HTTP 400 (BadRequest) -> Signing credentials were invalid.

Username requirements: min 4, max 20 symbols; only ASCII letters or digits.
Password requirements: min 5, max 19 symbols.
```

---

### Reset Token Endpoints

#### `POST /api/reset:`
```
Accepts: application/x-www-form-urlencoded
Body:

email=string (email)

Returns:

HTTP 200 (OK) -> Token was successfully requested.
HTTP 404 (NotFound) -> User with such email doesn't exist.
HTTP 400 (BadRequest) -> Given Email had invalid format.
HTTP 409 (Conflict) -> Current User already requested his Reset Token.
```

--- 

#### `POST /api/reset/{token:string}:`
```
Accepts: application/x-www-form-urlencoded
Body:

newPassword=string? & confirmPassword=string?

Returns:

HTTP 200 (OK) -> If new Password is provided, user successfully changed its Password. Else, Given Reset Token for User is Exists.
HTTP 404 (NotFound) -> Given Reset Token is not exists or already has expired.
HTTP 400 (BadRequest) -> New Password and Confirm Password are Not Same, or have invalid format.

Password requirements: min 5, max 19 symbols.
```
