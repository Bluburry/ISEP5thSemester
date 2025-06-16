export interface LoginResponse {
    Result: string;
    Token: string;
    Type: string;
}

export interface LoginCredentialsDto {
    username: string;
    password: string;
}

export interface UserDto {
    EmailAddress: string;
    Role: string;
    ActivationStatus: string;
}
