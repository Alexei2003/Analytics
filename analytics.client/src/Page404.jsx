import React from 'react';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';

const NotFoundPage = () => {
    const navigate = useNavigate();

    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
    }, []);


    return (
        <>
            <div>
                <h1>404 - Страница не найдена</h1>
                <p>Извините, запрашиваемая страница не существует.</p>
            </div>
        </>
    );
};

export default NotFoundPage;