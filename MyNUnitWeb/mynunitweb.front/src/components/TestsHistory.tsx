import React, {useState, useEffect} from 'react';
import {AllFileTestResultsViewModel} from '../api'
import ApiSingleton from "../api/ApiSingleton";
import {TestsResult} from "./TestsResult";

interface ITestsHistoryState {
    testResults: AllFileTestResultsViewModel | undefined,
}

export const TestsHistory: React.FC = () => {
    const [testHistory, setTestHistory] = useState<ITestsHistoryState>({
        testResults: undefined,
    })
    
    const getTestHistory = async () => {
        const testHistory = await ApiSingleton.testsApi.apiTestsTestHistoryGet()
        
        setTestHistory(() => ({testResults: testHistory}))
    }
    
    return <div>
        <TestsResult
            results={testHistory.testResults!}
        />
    </div>
};