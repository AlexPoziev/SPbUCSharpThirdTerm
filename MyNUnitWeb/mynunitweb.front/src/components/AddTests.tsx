import {TextField, Typography, Button, Grid} from "@mui/material"
import ApiSingleton from "../api/ApiSingleton";
import React from 'react'
import { MuiFileInput } from 'mui-file-input';

interface IAddFilesState {
    
}

export const AddTests = () => {
    
    const [file, setFile] = React.useState(null)
    const submitFiles = async (e: File[]) => {
        const decoder = new TextDecoder("utf-8")
        
        const temp: string[] = [];
        
        console.log(temp)
        
        e.forEach(async f => {
            temp.push((decoder.decode(await f.arrayBuffer())))
        })
        
        ApiSingleton.testsApi.apiTestsTestBytesPost(temp);
    }
    
    return <div style={{marginTop: "20px"}}>
        <Typography variant="h6">
            Выберите .dll файлы сборок
        </Typography>
        <MuiFileInput multiple onChange={submitFiles}/>
    </div>
};