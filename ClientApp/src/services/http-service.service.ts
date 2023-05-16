import {HttpClient, HttpParams} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface IHttpParameters {
  [key: string]: string | string[] | number;
}

@Injectable({ providedIn: 'root' })
export class HttpService {
  constructor(private httpClient: HttpClient) {}

  private mapRouteParametersToUrl(url : URL, queryParameters?: IHttpParameters) {
    let resultUrl : string = url.toString();
    for (const key in queryParameters) {
      resultUrl = resultUrl.concat(`/${queryParameters[key]}`);
    }

    return resultUrl;
  }

  private mapQueryParametersToHttpParams(queryParameters? : IHttpParameters) {
    let mappedParams = new HttpParams();
    for (const key in queryParameters) {
      mappedParams = mappedParams.set(key, String(queryParameters[key]));
    }

    return mappedParams;
  }

  private mapRouteAndQueryParameters(url: URL, routeParameters?: IHttpParameters, queryParameters?: IHttpParameters) : [string, HttpParams] {
    return [this.mapRouteParametersToUrl(url, routeParameters), this.mapQueryParametersToHttpParams(queryParameters)];
  }

  public get<T>(url: URL, routeParameters?: IHttpParameters, queryParameters?: IHttpParameters): Observable<T> {
    const [mappedUrl, mappedQueryParams] = this.mapRouteAndQueryParameters(url, routeParameters, queryParameters);

    return this.httpClient.get<T>(mappedUrl, { params: mappedQueryParams });
  }

  public post<T>(
    url: URL,
    body?: any,
    routeParameters?: IHttpParameters,
    queryParameters?: IHttpParameters
  ): Observable<T> {
    const [mappedUrl, mappedQueryParams] = this.mapRouteAndQueryParameters(url, routeParameters, queryParameters);

    return this.httpClient.post<T>(mappedUrl, body, { params: mappedQueryParams });
  }

  public put<T>(
    url: URL,
    body?: any,
    routeParameters?: IHttpParameters,
    queryParameters?: IHttpParameters
  ): Observable<T> {
    const [mappedUrl, mappedQueryParams] = this.mapRouteAndQueryParameters(url, routeParameters, queryParameters);

    return this.httpClient.put<T>(mappedUrl, body,{ params: mappedQueryParams });
  }

  public delete<T>(
    url: URL,
    body?: any,
    routeParameters?: IHttpParameters,
    queryParameters?: IHttpParameters
  ): Observable<T> {
    const [mappedUrl, mappedQueryParams] = this.mapRouteAndQueryParameters(url, routeParameters, queryParameters);

    return this.httpClient.delete<T>(mappedUrl, {
      body: body,
      params: mappedQueryParams,
    });
  }
}
