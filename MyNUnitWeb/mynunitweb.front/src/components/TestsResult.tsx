import { TreeView, TreeItem } from "@mui/x-tree-view"
import {AllFileTestResultsViewModel} from '../api'
import React, {useState, useEffect} from 'react';
import {CheckCircle, Cancel, NotificationsPaused, Error} from '@mui/icons-material'
import { Typography, Box } from '@mui/material'

interface ITestsResultProps {
    results: AllFileTestResultsViewModel | undefined,
}

export const TestsResult: React.FC<ITestsResultProps> = (props) => {
    console.log(props)
    let totalValue = 0;
    
    return <div>
        <Box marginTop={3} maxWidth={1000}>
        <TreeView>
            {props.results?.fileTestResults?.map((fr, index) => 
                <TreeItem 
                    nodeId={(totalValue++).toString()} 
                    label={<Typography><b>Assembly</b>: {fr.name} | {fr.testDuration} ms</Typography>} 
                    icon={fr.isIgnored ? <NotificationsPaused style={{fill: "blue"}}/> : fr.isPassed ? <CheckCircle style={{fill: "green"}}/> : <Cancel style={{fill: "red"}}/>} >
                    {fr.classTestResults!.map((cr, crIndex) =>
                        <TreeItem nodeId={(totalValue++).toString()}
                                  label={cr.validationErrors != undefined 
                                      ? <Typography><b>Class</b>: {cr.name} | <b>Error Reason</b>: {cr.validationErrors!.join(' | ')}</Typography>
                                      : <Typography><b>Class</b>: {cr.name} | {cr.testDuration} ms</Typography>}
                                  icon={cr.validationErrors != undefined 
                                      ? <Error style={{fill: "red"}}/> 
                                      : cr.isIgnored 
                                          ? <NotificationsPaused style={{fill: "blue"}}/> 
                                          : cr.isPassed
                                              ? <CheckCircle style={{fill: "green"}}/> 
                                              : <Cancel style={{fill: "red"}}/>} >
                            {cr.methodTestResults!.map((mr, mrIndex) => 
                            <TreeItem nodeId={(totalValue++).toString()}
                                      label={mr.isPassed 
                                          ? <Typography><b>Test</b>: {mr.name} | {mr.testDuration} ms</Typography> 
                                          : <Typography><b>Test</b>: {mr.name} | {mr.testDuration} ms | <b>Reason</b>: {mr.isFailed ? mr.failReasons : mr.ignoredReason}</Typography>}
                                      icon={mr.isIgnored ? <NotificationsPaused style={{fill: "blue"}}/> : mr.isPassed ? <CheckCircle style={{fill: "green"}}/> : <Cancel style={{fill: "red"}}/>} />
                            )}
                        </TreeItem>
                    )}
                </TreeItem>
            )}
        </TreeView>
        </Box>
    </div>
}