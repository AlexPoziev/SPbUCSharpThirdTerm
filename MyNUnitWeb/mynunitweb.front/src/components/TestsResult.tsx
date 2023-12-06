import { TreeView, TreeItem } from "@mui/x-tree-view"
import {AllFileTestResultsViewModel} from '../api'
import React, {useState, useEffect} from 'react';

interface ITestsResultProps {
    results: AllFileTestResultsViewModel,
}

export const TestsResult: React.FC<ITestsResultProps> = (props) => {
    const fileTestResultsCount = props.results.fileTestResults!.length;
    
    return <div>
        <TreeView>
            {props.results.fileTestResults!.map((fr, index) => 
                <TreeItem nodeId="1" label={fr.name}>
                    {fr.classTestResults!.map((cr, crIndex) =>
                        <TreeItem label={cr.name} nodeId="1">
                            {cr.methodTestResults!.map((mr, mrIndex) => 
                            <TreeItem label={mr.name} nodeId="1" />
                            )}
                        </TreeItem>
                    )}
                </TreeItem>
            )}
        </TreeView>
    </div>
}