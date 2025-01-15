import { useEffect, useState } from 'react';
import './Authorization.css';
import { useNavigate } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';

function Authorization() {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');

    const navigate = useNavigate();
    useEffect(() => {
        if (getUserRole()) {
            navigate('/listemployees');
        }
    }, []);

    const handleSetLogin = (event) => {
        setLogin(event.target.value);
    };

    const handleSetPassword = (event) => {
        setPassword(event.target.value);
    };

    return (
        <div className="container">
            <div className="auth-header">
                <h1> Авторизация </h1>
            </div>
            <div className="auth-input">
                <label> Логин </label>
                <input type="text" value={login} required onChange={handleSetLogin}></input>
            </div>
            <div className="auth-input">
                <label> Пароль </label>
                <input type="text" value={password} required onChange={handleSetPassword}></input>
            </div>
            <div className="auth-button">
                <button onClick={() => Login()}> Вход </button>
            </div>
        </div>
    );

    async function Login() {
        try {
            const response = await fetch('authorization', {
                method: 'POST',
                headers: {
                    "Content-type": "application/json"
                },
                body: JSON.stringify({
                    login: login,
                    password: password,
                }),
            })
            if (response.ok) {
                navigate('/listemployees');
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }
    }
    
}

export default Authorization;