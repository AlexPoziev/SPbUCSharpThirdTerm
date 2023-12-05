import {
    TestsApi
} from ".";

class Api {
    readonly testsApi: TestsApi;

    constructor(
        testsApi: TestsApi
    ) {
        this.testsApi = testsApi;
    }
}

const basePath = process.env.REACT_APP_BASE_PATH!

let ApiSingleton: Api;
ApiSingleton = new Api(
    new TestsApi({basePath: basePath})
);
export default ApiSingleton;
