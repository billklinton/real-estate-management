export interface BaseResponse<T> {
    statusCode: number;
    message: string;
    result: T;
}